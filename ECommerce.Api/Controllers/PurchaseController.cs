using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IPurchaseService purchaseService, ILogger<PurchaseController> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        [HttpPost("purchase")]
        [Authorize]
        public async Task<IActionResult> PurchaseProduct([FromBody] ProductPurchaseDto purchaseDto)
        {
            try
            {
                // JWT token'den UserId alıyoruz
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("Kullanıcı bilgisi JWT token'dan alınamadı.");
                    return Unauthorized("Kullanıcı bilgisi bulunamadı.");
                }

                var userId = int.Parse(userIdClaim.Value);

                var result = await _purchaseService.PurchaseProductAsync(userId, purchaseDto);

                if (result.Success)
                {
                    _logger.LogInformation($"Kullanıcı {userId} ürün satın alma işlemini başarıyla tamamladı: {purchaseDto.ProductId}");
                    return Ok(result.Message);
                }

                _logger.LogWarning($"Kullanıcı {userId} ürün satın alma işlemi başarısız oldu: {result.Message}");
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Satın alma işlemi sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}
