using ECommerce.Application.Common;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseService(
            IRepository<User> userRepository,
            IRepository<Product> productRepository,
            IRepository<Order> orderRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> PurchaseProductAsync(int userId, ProductPurchaseDto purchaseDto)
        {
            var product = await _productRepository.GetByIdAsync(purchaseDto.ProductId);
            if (product == null)
            {
                throw new BusinessException("Ürün bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new BusinessException("Kullanıcı bulunamadı.");
            }

            var totalPrice = product.Price * purchaseDto.Quantity;

            if (user.Balance < totalPrice)
            {
                throw new BusinessException("Yetersiz bakiye.");
            }

            // Kullanıcı bakiyesini güncelle
            user.Balance -= totalPrice;
            await _userRepository.UpdateAsync(user);


            // Yeni bir sipariş oluşturuluyor
            var order = new Order
            {
                UserId = userId,
                Status = "Hazırlanıyor",
                OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = purchaseDto.Quantity,
                    UnitPrice = product.Price
                }
            }
            };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            return Result.SuccessResult("Ürün başarıyla satın alındı.");
        }
    }
}