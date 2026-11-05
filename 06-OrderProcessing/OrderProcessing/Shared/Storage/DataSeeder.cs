
using OrderProcessing.Shared.Models;

namespace OrderProcessing.Shared.Storage;

public static class DataSeeder
{
    public static async Task ResetAsync(
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        // Delete the old Data folder
        if (Directory.Exists(DataFiles.DataDirectory))
        {
            Directory.Delete(
                DataFiles.DataDirectory,
                recursive: true);
        }

        // Create a fresh Data folder
        Directory.CreateDirectory(DataFiles.DataDirectory);

        // Create Phase 2 folders
        Directory.CreateDirectory(
            Path.Combine(DataFiles.DataDirectory,
                DataFiles.InvoicesFolder));

        Directory.CreateDirectory(
            Path.Combine(DataFiles.DataDirectory,
                DataFiles.ReportsFolder));

        // Create the store
        var store = new JsonFileStore();

        // Create seed data
        var customers = GetCustomers();
        var products = GetProducts();
        var coupons = GetCoupons();
        var incomingOrders = GetIncomingOrders();

        // Write all files through JsonFileStore
        await SaveAsync(store, DataFiles.Customers, customers, ct);
        await SaveAsync(store, DataFiles.Products, products, ct);
        await SaveAsync(store, DataFiles.Coupons, coupons, ct);
        await SaveAsync(store, DataFiles.IncomingOrders, incomingOrders, ct);

        // Empty files at the beginning
        await SaveAsync<Order>(
            store, DataFiles.Orders, new List<Order>(), ct);

        await SaveAsync<PaymentTransaction>(
            store, DataFiles.Payments,
            new List<PaymentTransaction>(), ct);

        await SaveAsync<EmailMessage>(
            store, DataFiles.Emails,
            new List<EmailMessage>(), ct);

        await SaveAsync<EmailMessage>(
            store, DataFiles.EmailRetry,
            new List<EmailMessage>(), ct);
    }

    private static async Task SaveAsync<T>(
        JsonFileStore store,
        string fileName,
        List<T> records,
        CancellationToken ct)
    {
        await store.WriteAllAsync(fileName, records, ct);

        Console.WriteLine(
            $"[Seeder] {fileName} -> {records.Count} records");
    }

    // =========================
    // CUSTOMERS
    // =========================

    private static List<Customer> GetCustomers()
    {
        return new List<Customer>
        {
            new() { Id = 1, Name = "Ahmed Ali",
                Email = "ahmed.ali@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Gold },

            new() { Id = 2, Name = "Sara Mohamed",
                Email = "sara.mohamed@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Silver },

            new() { Id = 3, Name = "Omar Hassan",
                Email = "omar.hassan@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Bronze },

            new() { Id = 4, Name = "Mona Ibrahim",
                Email = "mona.ibrahim@example.com",
                IsActive = false, LoyaltyTier = LoyaltyTier.Silver },

            new() { Id = 5, Name = "Youssef Adel",
                Email = "youssef.adel@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Gold },

            new() { Id = 6, Name = "Nour Khaled",
                Email = "nour.khaled@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Bronze },

            new() { Id = 7, Name = "Karim Samir",
                Email = "karim.samir@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Silver },

            new() { Id = 8, Name = "Laila Mahmoud",
                Email = "laila.mahmoud@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Gold },

            new() { Id = 9, Name = "Hassan Tarek",
                Email = "hassan.tarek@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Bronze },

            new() { Id = 10, Name = "Dina Fathy",
                Email = "dina.fathy@example.com",
                IsActive = true, LoyaltyTier = LoyaltyTier.Silver }
        };
    }

    // =========================
    // PRODUCTS
    // =========================

    private static List<Product> GetProducts()
    {
        return new List<Product>
        {
            new() { Id = 1, Name = "Laptop",
                Price = 1200.00m, StockQuantity = 10, WeightKg = 2.2m },

            new() { Id = 2, Name = "Smartphone",
                Price = 800.00m, StockQuantity = 15, WeightKg = 0.4m },

            new() { Id = 3, Name = "Wireless Mouse",
                Price = 25.00m, StockQuantity = 2, WeightKg = 0.1m },

            new() { Id = 4, Name = "Mechanical Keyboard",
                Price = 90.00m, StockQuantity = 1, WeightKg = 0.9m },

            new() { Id = 5, Name = "USB-C Hub",
                Price = 45.00m, StockQuantity = 30, WeightKg = 0.2m },

            new() { Id = 6, Name = "Headphones",
                Price = 150.00m, StockQuantity = 12, WeightKg = 0.3m },

            new() { Id = 7, Name = "27\" Monitor",
                Price = 350.00m, StockQuantity = 6, WeightKg = 5.0m },

            new() { Id = 8, Name = "Webcam",
                Price = 70.00m, StockQuantity = 20, WeightKg = 0.2m }
        };
    }

    // =========================
    // COUPONS
    // =========================

    private static List<Coupon> GetCoupons()
    {
        DateTime today = DateTime.Today;

        return new List<Coupon>
        {
            new()
            {
                Code = "SAVE10",
                DiscountPercent = 10,
                ExpiryDate = today.AddDays(180)
            },

            new()
            {
                Code = "WELCOME15",
                DiscountPercent = 15,
                ExpiryDate = today.AddDays(90)
            },

            new()
            {
                Code = "FLASH20",
                DiscountPercent = 20,
                ExpiryDate = today.AddDays(7)
            },

            new()
            {
                Code = "OLD20",
                DiscountPercent = 20,
                ExpiryDate = today.AddDays(-30)
            }
        };
    }

    // =========================
    // INCOMING ORDERS
    // =========================

    private static List<Order> GetIncomingOrders()
    {
        DateTime now = DateTime.Now;

        return new List<Order>
        {
            CreateOrder(1, 1, now,
                (1, 1), (3, 1), coupon: "SAVE10"),

            CreateOrder(2, 2, now,
                (2, 1)),

            CreateOrder(3, 3, now,
                (5, 2), (8, 1), coupon: "WELCOME15"),

            CreateOrder(4, 4, now,
                (6, 1)),

            CreateOrder(5, 5, now,
                (7, 1)),

            CreateOrder(6, 6, now,
                (5, 1), coupon: "FLASH20"),

            CreateOrder(7, 7, now,
                (6, 1), (8, 1)),

            CreateOrder(8, 8, now,
                (1, 1), coupon: "SAVE10"),

            CreateOrder(9, 9, now,
                (3, 5)),

            CreateOrder(10, 10, now,
                (4, 1)),

            CreateOrder(11, 1, now,
                (4, 1)),

            CreateOrder(12, 2, now,
                (2, 1), coupon: "OLD20"),

            CreateOrder(13, 3, now,
                (6, 2)),

            CreateOrder(14, 5, now,
                (8, 2), (5, 1), coupon: "WELCOME15"),

            CreateOrder(15, 99, now,
                (2, 1)),

            CreateOrder(16, 6, now,
                (7, 1), (5, 1)),

            CreateOrder(17, 7, now,
                (2, 1), coupon: "SAVE10"),

            CreateOrder(18, 8, now,
                (6, 1), coupon: "FAKE50"),

            CreateOrder(19, 9, now,
                (8, 1)),

            CreateOrder(20, 10, now,
                (1, 1), (6, 1))
        };
    }

    private static Order CreateOrder(
        int id,
        int customerId,
        DateTime createdAt,
        (int ProductId, int Quantity) item1,
        (int ProductId, int Quantity)? item2 = null,
        string? coupon = null)
    {
        var items = new List<OrderItem>
        {
            new()
            {
                ProductId = item1.ProductId,
                Quantity = item1.Quantity,
                UnitPrice = 0m
            }
        };

        if (item2.HasValue)
        {
            items.Add(new OrderItem
            {
                ProductId = item2.Value.ProductId,
                Quantity = item2.Value.Quantity,
                UnitPrice = 0m
            });
        }

        return new Order
        {
            Id = id,
            CustomerId = customerId,
            Items = items,
            CouponCode = coupon,
            SubTotal = 0m,
            Discount = 0m,
            ShippingCost = 0m,
            TotalAmount = 0m,
            Status = OrderStatus.Pending,
            CreatedAt = createdAt,
            FailureReason = null,
            TransactionId = null
        };
    }
}
