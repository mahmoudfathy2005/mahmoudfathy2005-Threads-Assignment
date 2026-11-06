namespace OrderProcessing.Shared.Models;

using System;
using System.Collections.Generic;

// =========================
// CUSTOMER
// =========================

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public LoyaltyTier LoyaltyTier { get; set; }
}

// =========================
// PRODUCT
// =========================

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public decimal WeightKg { get; set; }
}

// =========================
// COUPON
// =========================

public class Coupon
{
    public string Code { get; set; } = string.Empty;

    public int DiscountPercent { get; set; }

    public DateTime ExpiryDate { get; set; }
}

// =========================
// ORDER ITEM
// =========================

public class OrderItem
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}

// =========================
// ORDER
// =========================

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public string? CouponCode { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? TransactionId { get; set; }
}

// =========================
// PAYMENT TRANSACTION
// =========================

public class PaymentTransaction
{
    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }
}

// =========================
// PAYMENT RESULT
// =========================

public class PaymentResult
{
    public bool Success { get; set; }

    public string? TransactionId { get; set; }

    public PaymentStatus Status { get; set; }

    public string? Message { get; set; }
}

// =========================
// EMAIL MESSAGE
// =========================

public class EmailMessage
{
    public Guid Id { get; set; }

    public int OrderId { get; set; }

    public string To { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public int Attempts { get; set; }

    public string? LastError { get; set; }
}

// =========================
// REPORT
// =========================

public class Report
{
    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public int CompletedCount { get; set; }

    public int RejectedCount { get; set; }

    public int FailedCount { get; set; }

    public int PaymentFailedCount { get; set; }

    public string TopSellingProduct { get; set; } = string.Empty;

    public int TopSellingQuantity { get; set; }

    public decimal AverageOrderValue { get; set; }

    public DateTime GeneratedAt { get; set; }
}
