using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Utilities;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork, ILogger<AuthController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("create-token")]
        public async Task<IActionResult> CreateToken([FromBody] UserLoginRequest request)
        {
            try
            {
                // TCKN doğrulaması
                if (!TcknValidator.Validate(request.TCKN))
                {
                    _logger.LogWarning($"Hatalı TCKN girişi: {request.TCKN}");
                    return Unauthorized("Hatalı TCKN.");
                }

                var user = await _unitOfWork.Users.FindAsync(u => u.TCKN == request.TCKN);

                if (user == null)
                {
                    _logger.LogWarning($"Geçersiz TCKN giriş: {request.TCKN}");
                    return Unauthorized("Geçersiz TCKN.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    _logger.LogWarning($"Geçersiz şifre giriş: {request.TCKN}");
                    return Unauthorized("Geçersiz şifre.");
                }

                var token = _jwtTokenService.GenerateToken(user);
                _logger.LogInformation($"Başarılı giriş: {request.TCKN}");
                return Ok(new { token = "Bearer " + token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token oluşturulurken hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}