using OrderProcessing.Phase1.Repositories;
using OrderProcessing.Shared.Logging;
using OrderProcessing.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderProcessing.Phase1.Validation
{
    public class OrderValidator
    {
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;
        private readonly CouponRepository _couponRepository;

        public OrderValidator(CustomerRepository customerRepository ,ProductRepository productRepository , CouponRepository couponRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _couponRepository = couponRepository;
        }


        public async Task<Order> ValidateOrderAsync(Order order,CancellationToken ct)
        {
            const string source = "OrderValidator";

            Log.Info(source, $"Validating Order #{order.Id}");

            // Start with a clean result
            order.Status = OrderStatus.Pending;
            order.FailureReason = null; 

            var customer =await _customerRepository.GetByIdAsync(order.CustomerId,ct);

            if (customer == null) {
              return  Reject(order, $"Customer #{order.CustomerId} not found");
            }

            
            if (!customer!.IsActive)
            {
                return Reject( order,"Customer is inactive");
            }


            foreach (var item in order.Items) { 
            
                var product= await _productRepository.GetByIdAsync (item.ProductId,ct);
                if (product == null) {

                  return  Reject(order, $" Product #{item.ProductId} not found");
                }

                if (product!.StockQuantity < item.Quantity)
                {
                  return  Reject(order, $"Insufficient stock for '{product.Name}'");

                }           
            }


            // 5 and 6. Coupon
            if (!string.IsNullOrWhiteSpace(order.CouponCode))
            {
                var coupon = await _couponRepository.GetByCodeAsync(order.CouponCode,ct);

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
        private static Order Reject(Order order,string reason)
        {
            order.Status = OrderStatus.Rejected;
            order.FailureReason = reason;

            Log.Error("OrderValidator", $"Order #{order.Id} rejected: {reason}");


            return order;
        }


    }
}
