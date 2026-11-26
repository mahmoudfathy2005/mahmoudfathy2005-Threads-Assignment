
using System.Diagnostics;

namespace InvoiceExport;

public sealed class InvoiceExportService
{
    readonly OrderDatabase _db;
    readonly CustomerFileStore _customers;
    readonly InvoiceFileWriter _invoices;

    public InvoiceExportService(
        OrderDatabase db,
        CustomerFileStore customers,
        InvoiceFileWriter invoices)
    {
        _db = db;
        _customers = customers;
        _invoices = invoices;
    }

    public async Task ExportAllPendingAsync(
        CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();

        List<Order> orders =
            await _db.GetPendingOrdersAsync(cancellationToken);

        await Parallel.ForEachAsync(
            orders,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 8,
                CancellationToken = cancellationToken
            },
            async (order, token) =>
            {
                Order fresh = await _db.GetOrderByIdAsync(
                    order.Id, token)
                    ?? throw new InvalidOperationException(
                        $"Order {order.Id} missing");

                CustomerProfile customer =
                    await _customers.ReadProfileAsync(
                        fresh.CustomerId, token);

                string path =
                    await _invoices.WriteInvoiceAsync(
                        fresh, customer, token);

                Console.WriteLine($"Wrote {path}");
            });

        watch.Stop();

        Console.WriteLine(
            $"Export finished in {watch.Elapsed.TotalSeconds:F1} s");
    }
}
