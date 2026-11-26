using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class SyncCouponRepository
{
    private readonly JsonFileStore _store;

    public SyncCouponRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Coupon? GetByCode(string code)
    {
        const string source = "SyncCouponRepository";

        SyncDelayHelper.Wait(
            source, "Opening connection", 200);

        SyncDelayHelper.Wait(
            source, "Executing query",
            Random.Shared.Next(300, 601));

        List<Coupon> coupons =
            _store.ReadAll<Coupon>(DataFiles.Coupons);

        return coupons.FirstOrDefault(c =>
            string.Equals(
                c.Code,
                code,
                StringComparison.OrdinalIgnoreCase));
    }
}