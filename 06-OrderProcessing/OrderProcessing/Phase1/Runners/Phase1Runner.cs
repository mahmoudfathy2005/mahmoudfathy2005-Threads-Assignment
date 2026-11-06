using System.Diagnostics;

using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

using OrderProcessing.Phase1.Repositories;
using OrderProcessing.Phase1.Validation;

namespace OrderProcessing.Phase1.Runners;

public class Phase1Runner
{
    public async Task RunAsync(CancellationToken ct)
    {
        var store = new JsonFileStore();

        Console.WriteLine();
        Console.WriteLine("========== PHASE 1 ==========");
        Console.WriteLine();

        // =================================
        // 1. RESET AND LOAD INCOMING ORDERS
        // =================================

        await DataSeeder.ResetAsync(ct);

        var incomingRepository =
            new IncomingOrderRepository(store);

        List<Order> incomingOrders =
            await incomingRepository.GetAllAsync(ct);

        Console.WriteLine();
        Console.WriteLine(
            $"Loaded {incomingOrders.Count} incoming orders");

        // =================================
        // 2. SYNC VALIDATION
        // =================================

        var syncValidator = new SyncOrderValidator(
            new SyncCustomerRepository(store),
            new SyncProductRepository(store),
            new SyncCouponRepository(store));

        Console.WriteLine();
        Console.WriteLine("========== SYNC RUN ==========");

        Stopwatch syncWatch = Stopwatch.StartNew();

        foreach (Order order in incomingOrders)
        {
            syncValidator.ValidateOrder(order);
        }

        syncWatch.Stop();

        TimeSpan syncTime = syncWatch.Elapsed;

        Console.WriteLine();
        Console.WriteLine(
            $"Sync Time: {syncTime.TotalSeconds:F1} seconds");

        // =================================
        // 3. RESET AGAIN
        // =================================

        await DataSeeder.ResetAsync(ct);

        // Reload incoming orders from the reset data
        incomingOrders =
            await incomingRepository.GetAllAsync(ct);

        // =================================
        // 4. ASYNC VALIDATION AND SAVE
        // =================================

        var customerRepository =
            new CustomerRepository(store);

        var productRepository =
            new ProductRepository(store);

        var couponRepository =
            new CouponRepository(store);

        var orderRepository =
            new OrderRepository(store);

        var validator = new OrderValidator(
            customerRepository,
            productRepository,
            couponRepository);

        Console.WriteLine();
        Console.WriteLine("========== ASYNC RUN ==========");

        Stopwatch asyncWatch = Stopwatch.StartNew();

        foreach (Order order in incomingOrders)
        {
            ct.ThrowIfCancellationRequested();

            await validator.ValidateOrderAsync(order, ct);

            await orderRepository.SaveAsync(order, ct);
        }

        asyncWatch.Stop();

        TimeSpan asyncTime = asyncWatch.Elapsed;

        Console.WriteLine();
        Console.WriteLine(
            $"Async Time: {asyncTime.TotalSeconds:F1} seconds");

        // =================================
        // 5. READ RESULTS FROM JSON FILE
        // =================================

        List<Order> validatedOrders =
            await orderRepository.GetByStatusAsync(
                OrderStatus.Validated, ct);

        List<Order> rejectedOrders =
            await orderRepository.GetByStatusAsync(
                OrderStatus.Rejected, ct);

        Console.WriteLine();
        Console.WriteLine("===== Validation Summary =====");

        Console.WriteLine(
            $"Total Orders: {validatedOrders.Count + rejectedOrders.Count}");

        Console.WriteLine(
            $"Validated:    {validatedOrders.Count}");

        Console.WriteLine(
            $"Rejected:     {rejectedOrders.Count}");

        foreach (Order order in rejectedOrders.OrderBy(o => o.Id))
        {
            Console.WriteLine(
                $"  - Order #{order.Id}: {order.FailureReason}");
        }

        Console.WriteLine(
            $"Sync Time:  {syncTime.TotalSeconds:F1} seconds");

        Console.WriteLine(
            $"Async Time: {asyncTime.TotalSeconds:F1} seconds");

        Console.WriteLine();
        Console.WriteLine(
            "orders.json now contains " +
            $"{validatedOrders.Count + rejectedOrders.Count} records.");

        // =================================
        // 6. CHECK EXPECTED RESULT
        // =================================

        if (validatedOrders.Count == 15 &&
            rejectedOrders.Count == 5)
        {
            Log.Success(
                "Phase1Runner",
                "Phase 1 completed successfully");
        }
        else
        {
            Log.Error(
                "Phase1Runner",
                "Unexpected validation result. Check your code.");
        }
    }
}