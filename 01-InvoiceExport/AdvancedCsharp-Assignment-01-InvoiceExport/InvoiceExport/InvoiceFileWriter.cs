
namespace InvoiceExport;

public sealed class InvoiceFileWriter
{
    readonly string _folder;

    public InvoiceFileWriter(string folder) => _folder = folder;

    public async Task<string> WriteInvoiceAsync(
        Order order,
        CustomerProfile customer,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_folder);

        string path = Path.Combine(
            _folder, $"invoice-{order.Id}.txt");

        string body =
            $"""
            SwiftBite Invoice
            -----------------
            Order:      {order.Id}
            Restaurant: {order.Restaurant}
            Customer:   {customer.Name} <{customer.Email}>
            Total:      {order.Total:C}
            """;

        await Task.Delay(300, cancellationToken);

        await File.WriteAllTextAsync(
            path, body, cancellationToken);

        return path;
    }
}
