using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ECommerceDbContext _context;

        public UserRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByTcknAsync(string tckn)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.TCKN == tckn);
        }
    }
}