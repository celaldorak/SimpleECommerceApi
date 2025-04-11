using ECommerce.Domain.Entities;
using ECommerce.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure
{

    public class DbInitializer
    {
        private readonly ECommerceDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public DbInitializer(ECommerceDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task Initialize()
        {
            await _context.Database.MigrateAsync();

            // Eğer User tablosunda hiç kayıt yoksa, admin kullanıcısını ekle
            if (!_context.Users.Any())
            {
                var adminUser = new User
                {
                    TCKN = "11111111111",
                    FullName = "Admin",
                    Role = "Admin",
                    Balance = 0,
                    Orders = new List<Order>()
                };

                // Şifreyi hash'le
                var hashedPassword = _passwordHasher.HashPassword(adminUser, "1234");
                adminUser.PasswordHash = hashedPassword;

                await _context.Users.AddAsync(adminUser);
                await _context.SaveChangesAsync();
            }
        }
    }

}
