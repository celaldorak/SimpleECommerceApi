using ECommerce.Application.Common;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services
{
    public interface IPurchaseService
    {
        Task<Result> PurchaseProductAsync(int userId, ProductPurchaseDto purchaseDto);
    }
}