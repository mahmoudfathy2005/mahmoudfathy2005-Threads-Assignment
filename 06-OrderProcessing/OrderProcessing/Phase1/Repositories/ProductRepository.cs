using OrderProcessing.Shared.Logging;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Phase1.Repositories
{
    public class ProductRepository
    {
        private readonly JsonFileStore _store;

        public ProductRepository(JsonFileStore store)
        {
            _store = store;
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken ct)
        {
            string sourceName = "ProductRepository";

            await DelayHelper.WaitAsync(sourceName, "Opening Connection ", 200, ct);

            await DelayHelper.WaitAsync(sourceName, "Execute Query", Random.Shared.Next(300, 601), ct);

            var products = await _store.ReadAllAsync<Product>(DataFiles.Products, ct);

            Product? product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                Log.Error(sourceName, $"Product #{id} not found");
            else
                Log.Success(sourceName, $"Product #{id} found ({product.Name})");

            return product!;
        }


        public async Task<bool> ReserveStockAsync(int productId, int qty, CancellationToken ct)
        {
            const string source = "ProductRepository";

            if (qty <= 0)
                throw new ArgumentOutOfRangeException(nameof(qty));

            await DelayHelper.WaitAsync(source, "Opening connection ", 200, ct);

            await DelayHelper.WaitAsync(source, "Beging transaction ", 100, ct);

            await DelayHelper.WaitAsync(source, "Inserting / Updating Product", 400, ct);

            bool reserved = await _store.UpdateAsync<Product, bool>(DataFiles.Products,
                products =>
                {
                    var product = products.FirstOrDefault(p => p.Id == productId);

                    if (product == null || product.StockQuantity < qty)
                        return false;
                    product.StockQuantity -= qty;

                    return true;
                },
                ct
            );

            await DelayHelper.WaitAsync(
             source, "Committing transaction", 200, ct);

            if (reserved)
                Log.Success(source,
                    $"Reserved {qty} units of Product #{productId}");
            else
                Log.Error(source,
                    $"Could not reserve Product #{productId}");

            return reserved;

        }

    }
}
