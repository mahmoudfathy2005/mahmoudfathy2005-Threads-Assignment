using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OrderProcessing.Shared.Storage;

public static class DataFiles
{
    public static readonly string DataDirectory =
        Path.Combine(AppContext.BaseDirectory, "Data");

    public const string Customers = "customers.json";
    public const string Products = "products.json";
    public const string Coupons = "coupons.json";
    public const string IncomingOrders = "incoming_orders.json";
    public const string Orders = "orders.json";
    public const string Payments = "payments.json";
    public const string Emails = "emails.json";
    public const string EmailRetry = "email_retry.json";

    public const string InvoicesFolder = "invoices";
    public const string ReportsFolder = "reports";

    public static string GetFilePath(string fileName)
    {
        return Path.Combine(DataDirectory, fileName);
    }
}
