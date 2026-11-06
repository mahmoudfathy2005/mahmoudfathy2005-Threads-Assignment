using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Logging;
using OrderProcessing.Phase1.Repositories;

namespace OrderProcessing.Phase1.Validation;

public class SyncOrderValidator
{
    private readonly SyncCustomerRepository _customers;
    private readonly SyncProductRepository _products;
    private readonly SyncCouponRepository _coupons;

    public SyncOrderValidator(
        SyncCustomerRepository customers,
        SyncProductRepository products,
        SyncCouponRepository coupons)
    {
        _customers = customers;
        _products = products;
        _coupons = coupons;
    }

    public Order ValidateOrder(Order order)
    {
        const string source = "SyncOrderValidator";

        Log.Info(source, $"Validating Order #{order.Id}");

        order.Status = OrderStatus.Pending;
        order.FailureReason = null;

        // 1. Customer exists
        Customer? customer =
            _customers.GetById(order.CustomerId);

        if (customer == null)
        {
            return Reject(
                order,
                $"Customer #{order.CustomerId} not found");
        }

        // 2. Customer is active
        if (!customer.IsActive)
        {
            return Reject(
                order,
                "Customer is inactive");
        }

        // 3 and 4. Products and stock
        foreach (OrderItem item in order.Items)
        {
            Product? product =
                _products.GetById(item.ProductId);

            if (product == null)
            {
                return Reject(
                    order,
                    $"Product #{item.ProductId} not found");
            }

            if (product.StockQuantity < item.Quantity)
            {
                return Reject(
                    order,
                    $"Insufficient stock for '{product.Name}'");
            }
        }

        // 5 and 6. Coupon
        if (!string.IsNullOrWhiteSpace(order.CouponCode))
        {
            Coupon? coupon =
                _coupons.GetByCode(order.CouponCode);

            if (coupon == null)
            {
                return Reject(
                    order,
                    $"Coupon '{order.CouponCode}' does not exist");
            }

            if (coupon.ExpiryDate.Date < DateTime.Today)
            {
                return Reject(
                    order,
                    $"Coupon '{order.CouponCode}' has expired");
            }
        }

        order.Status = OrderStatus.Validated;
        order.FailureReason = null;

        Log.Success(source,
            $"Order #{order.Id} validated");

        return order;
    }

    private static Order Reject(
        Order order,
        string reason)
    {
        order.Status = OrderStatus.Rejected;
        order.FailureReason = reason;

        Log.Error(
            "SyncOrderValidator",
            $"Order #{order.Id} rejected: {reason}");

        return order;
    }
}