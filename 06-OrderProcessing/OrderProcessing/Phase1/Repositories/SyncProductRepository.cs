using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class SyncProductRepository
{
    private readonly JsonFileStore _store;

    public SyncProductRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Product? GetById(int id)
    {
        const string source = "SyncProductRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Executing query",
            Random.Shared.Next(300, 601));

        List<Product> products =
            _store.ReadAll<Product>(DataFiles.Products);

        return products.FirstOrDefault(p => p.Id == id);
    }

    public bool ReserveStock(int productId, int qty)
    {
        const string source = "SyncProductRepository";

        if (qty <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty));

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Beginning transaction", 100);

        SyncDelayHelper.Wait(
            source, "Inserting / updating product", 400);

        bool reserved =
            _store.Update<Product, bool>(
                DataFiles.Products,
                products =>
                {
                    Product? product =
                        products.FirstOrDefault(
                            p => p.Id == productId);

                    if (product == null ||
                        product.StockQuantity < qty)
                    {
                        return false;
                    }

                    product.StockQuantity -= qty;
                    return true;
                });

        SyncDelayHelper.Wait(
            source, "Committing transaction", 200);

        return reserved;
    }
}