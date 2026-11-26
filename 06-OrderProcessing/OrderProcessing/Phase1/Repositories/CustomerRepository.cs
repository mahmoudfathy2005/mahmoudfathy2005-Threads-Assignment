using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class CustomerRepository
{
    private readonly JsonFileStore _Store;

    public CustomerRepository(JsonFileStore store)
    {
        _Store = store;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct)
    {
        string sourceName = "CustomerRepository";

        await DelayHelper.WaitAsync(sourceName, "Opning Conection ", 200, ct);

        await DelayHelper.WaitAsync(sourceName, "Execute Query", Random.Shared.Next(300, 601), ct);

        List<Customer> customers = await _Store.ReadAllAsync<Customer>(DataFiles.Customers, ct);

        Customer? customer = customers.FirstOrDefault(c => c.Id == id);

        if (customer == null)
        {
            Log.Error(sourceName, $"Customer #{id} not found");

        }
        else
        {
            Log.Success(sourceName, $"Customer #{id} found ({customer.Name})");
        }

        return customer;


    }

}