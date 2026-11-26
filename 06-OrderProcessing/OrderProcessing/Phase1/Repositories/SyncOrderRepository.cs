using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class SyncOrderRepository
{
    private readonly JsonFileStore _store;

    public SyncOrderRepository(JsonFileStore store)
    {
        _store = store;
    }

    public void Save(Order order)
    {
        const string source = "SyncOrderRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Beginning transaction", 100);

        SyncDelayHelper.Wait(
            source, "Inserting / updating order", 400);

        _store.Update<Order, bool>(
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
            });

        SyncDelayHelper.Wait(
            source, "Committing transaction", 200);
    }

    public List<Order> GetAll()
    {
        const string source = "SyncOrderRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Executing query",
            Random.Shared.Next(300, 601));

        return _store.ReadAll<Order>(DataFiles.Orders);
    }

    public List<Order> GetByStatus(OrderStatus status)
    {
        const string source = "SyncOrderRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Executing query",
            Random.Shared.Next(300, 601));

        return _store.ReadAll<Order>(DataFiles.Orders)
            .Where(o => o.Status == status)
            .ToList();
    }
}