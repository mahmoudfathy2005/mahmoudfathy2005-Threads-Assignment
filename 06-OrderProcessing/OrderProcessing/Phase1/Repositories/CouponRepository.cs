using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;
using OrderProcessing.Shared.Logging;

namespace OrderProcessing.Phase1.Repositories;

public class CouponRepository
{
    private readonly JsonFileStore _store;

    public CouponRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken ct)
    {
        const string source = "CouponRepository";

        await DelayHelper.WaitAsync(
            source, "Opening connection", 200, ct);

        await DelayHelper.WaitAsync(
            source, "Executing query",
            Random.Shared.Next(300, 601), ct);

        List<Coupon> coupons =
            await _store.ReadAllAsync<Coupon>(
                DataFiles.Coupons, ct);

        Coupon? coupon =
            coupons.FirstOrDefault(c =>
                string.Equals(
                    c.Code,
                    code,
                    StringComparison.OrdinalIgnoreCase));

        if (coupon == null)
            Log.Error(source, $"Coupon '{code}' not found");
        else
            Log.Success(source, $"Coupon '{code}' found");

        return coupon;
    }
}