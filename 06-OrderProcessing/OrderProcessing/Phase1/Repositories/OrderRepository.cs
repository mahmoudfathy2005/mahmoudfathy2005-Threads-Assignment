using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class OrderRepository
{
    private readonly JsonFileStore _store;

    public OrderRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task SaveAsync(
        Order order,
        CancellationToken ct)
    {
        const string source = "OrderRepository";

        await DelayHelper.WaitAsync(
            source, "Opening connection", 200, ct);

        await DelayHelper.WaitAsync(
            source, "Beginning transaction", 100, ct);

        await DelayHelper.WaitAsync(
            source, "Inserting / updating order", 400, ct);

        await _store.UpdateAsync<Order, bool>(
            DataFiles.Orders,
            orders =>
            {
                int index =
                    orders.FindIndex(o => o.Id == order.Id);

                if (index >= 0)
                    orders[index] = order;
                else
                    orders.Add(order);

                return true;
            },
            ct);

        await DelayHelper.WaitAsync(
            source, "Committing transaction", 200, ct);

        Log.Success(source, $"Order #{order.Id} saved");
    }

    public async Task<List<Order>> GetAllAsync(
        CancellationToken ct)
    {
        const string source = "OrderRepository";

        await DelayHelper.WaitAsync(
            source, "Opening connection", 200, ct);

        await DelayHelper.WaitAsync(
            source, "Executing query",
            Random.Shared.Next(300, 601), ct);

        List<Order> orders =
            await _store.ReadAllAsync<Order>(
                DataFiles.Orders, ct);

        Log.Success(source,
            $"Loaded {orders.Count} orders");

        return orders;
    }

    public async Task<List<Order>> GetByStatusAsync(
        OrderStatus status,
        CancellationToken ct)
    {
        const string source = "OrderRepository";

        await DelayHelper.WaitAsync(
            source, "Opening connection", 200, ct);

        await DelayHelper.WaitAsync(
            source, "Executing query",
            Random.Shared.Next(300, 601), ct);

        List<Order> orders =
            await _store.ReadAllAsync<Order>(
                DataFiles.Orders, ct);

        return orders
            .Where(o => o.Status == status)
            .ToList();
    }
}