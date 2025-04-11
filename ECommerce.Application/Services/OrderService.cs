using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ECommerceDbContext _context;


        public OrderService(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchasedProductDto>> GetPurchasedProductsByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                 .Where(o => o.UserId == userId)
                 .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                 .ToListAsync();

            var products = orders
                .SelectMany(o => o.OrderItems)
                .Select(oi => new PurchasedProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    Description = oi.Product.Description,
                    Price = oi.Product.Price

                })
                .ToList();

            return products;
        }

        public async Task UpdateOrderStatusesAsync()
        {
            //"Hazırlanıyor" durumundaki siparişleri bul
            var orders = await _context.Orders
                .Where(o => o.Status == "Preparing")
                .ToListAsync();

            //Siparişlerin durumunu "Tamamlandı" olarak güncelle
            foreach (var order in orders)
            {
                order.Status = "Tamamlandı";
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();
        }
    }

}
