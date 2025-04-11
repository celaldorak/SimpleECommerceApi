using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<PurchasedProductDto>> GetPurchasedProductsByUserIdAsync(int userId);
        Task UpdateOrderStatusesAsync();
    }
}