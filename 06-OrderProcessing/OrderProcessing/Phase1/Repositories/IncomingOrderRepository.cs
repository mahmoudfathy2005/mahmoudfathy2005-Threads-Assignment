using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class IncomingOrderRepository
{
    private readonly JsonFileStore _store;

    public IncomingOrderRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<List<Order>> GetAllAsync(
        CancellationToken ct)
    {
        const string source = "IncomingOrderRepository";

        await DelayHelper.WaitAsync(
            source, "Opening connection", 200, ct);

        await DelayHelper.WaitAsync(
            source, "Executing query",
            Random.Shared.Next(300, 601), ct);

        List<Order> orders =
            await _store.ReadAllAsync<Order>(
                DataFiles.IncomingOrders, ct);

        Log.Success(source,
            $"Loaded {orders.Count} incoming orders");

        return orders;
    }
}