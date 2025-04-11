using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Utilities;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace ECommerce.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<string> RegisterUserAsync(string tckn, string password, string fullName, string role)
        {
            if (!TcknValidator.Validate(tckn))
            {
                throw new BusinessException("Geçersiz TCKN");
            }

            // TCKN'yi veritabanında kontrol et
            var existingUser = await _unitOfWork.Users.GetUserByTcknAsync(tckn);

            if (existingUser != null)
            {
                throw new BusinessException("Bu TCKN ile zaten bir kullanıcı mevcut.");
            }

            // Yeni bir kullanıcı nesnesi oluşturuluyor
            var user = new User
            {
                TCKN = tckn,
                FullName = fullName,
                Balance = 0,
                Role = role
            };

            // Şifreyi hashle
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Kullanıcıyı veritabanına ekle
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Başarı mesajını döndür
            return "Kullanıcı başarıyla kaydedildi.";
        }
    }
}