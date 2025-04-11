using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterRequest request)
        {
            try
            {
                _logger.LogInformation($"Kullanıcı kaydı başlatıldı: {request.FullName} ({request.TCKN})");

                var user = await _userService.RegisterUserAsync(request.TCKN, request.Password, request.FullName, "Customer");

                _logger.LogInformation($"Kullanıcı kaydı başarıyla tamamlandı: {request.FullName} ({request.TCKN})");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kullanıcı kaydı sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }

        // Admin kullanıcısı kaydı
        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> CreateAdmin([FromBody] UserRegisterRequest request)
        {
            try
            {
                _logger.LogInformation($"Admin kullanıcısı kaydı başlatıldı: {request.FullName} ({request.TCKN})");

                var admin = await _userService.RegisterUserAsync(request.TCKN, request.Password, request.FullName, "Admin");

                _logger.LogInformation($"Admin kullanıcısı kaydı başarıyla tamamlandı: {request.FullName} ({request.TCKN})");

                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Admin kaydı sırasında hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}
