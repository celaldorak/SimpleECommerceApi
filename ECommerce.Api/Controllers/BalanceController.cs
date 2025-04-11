using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BalanceController> _logger;

        public BalanceController(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<BalanceController> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Kullanıcının bakiyesini görüntüleme
        [HttpGet("get-balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    _logger.LogWarning("Geçersiz kullanıcı ID.");
                    return Unauthorized("Geçersiz kullanıcı.");
                }

                var user = await _userRepository.GetByIdAsync(Convert.ToInt32(userId));

                if (user == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı. Kullanıcı ID: {userId}");
                    return NotFound("Kullanıcı bulunamadı.");
                }

                _logger.LogInformation($"Kullanıcı {userId} bakiyesi görüntülendi.");
                return Ok(new { user.Balance });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBalance işlemi sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }

        // Kullanıcının bakiyesini yükleme
        [HttpPost("add-balance")]
        public async Task<IActionResult> AddBalance([FromBody] decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    _logger.LogWarning($"Geçersiz bakiye yükleme tutarı: {amount}");
                    return BadRequest("Bakiye yükleme tutarı sıfırdan büyük olmalıdır.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _logger.LogWarning("Geçersiz kullanıcı ID.");
                    return Unauthorized("Geçersiz kullanıcı.");
                }

                var user = await _userRepository.GetByIdAsync(Convert.ToInt32(userId));

                if (user == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı. Kullanıcı ID: {userId}");
                    return NotFound("Kullanıcı bulunamadı.");
                }

                // Bakiye ekleniyor
                user.Balance += amount;

                // Kullanıcıyı güncelle
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Kullanıcı {userId} bakiyesi {amount} TL arttırıldı. Yeni bakiye: {user.Balance}");

                return Ok(new { user.Balance });
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddBalance işlemi sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}
