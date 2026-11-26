using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class SyncCustomerRepository
{
    private readonly JsonFileStore _store;

    public SyncCustomerRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Customer? GetById(int id)
    {
        const string source = "SyncCustomerRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Executing query",
            Random.Shared.Next(300, 601));

        List<Customer> customers =
            _store.ReadAll<Customer>(DataFiles.Customers);

        Customer? customer =
            customers.FirstOrDefault(c => c.Id == id);

        return customer;
    }
}