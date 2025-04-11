using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // Kullanıcının satın aldığı ürünleri listeleme
        [HttpGet("my-products")]
        [Authorize]
        public async Task<IActionResult> GetUserPurchasedProducts()
        {
            try
            {
                // JWT'den kullanıcı ID'sini alıyoruz
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Geçersiz kullanıcı kimliği girişiminde bulunuldu.");
                    return Unauthorized("Kullanıcı kimliği alınamadı.");
                }

                // Servis üzerinden satın alınan ürünleri çekiyoruz
                var products = await _orderService.GetPurchasedProductsByUserIdAsync(userId);

                if (products == null || !products.Any())
                {
                    _logger.LogInformation($"Kullanıcı {userId} tarafından satın alınan ürün bulunamadı.");
                    return NotFound("Satın alınan ürün bulunamadı.");
                }

                _logger.LogInformation($"Kullanıcı {userId} tarafından satın alınan {products.Count} ürün listelendi.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetUserPurchasedProducts işlemi sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}
