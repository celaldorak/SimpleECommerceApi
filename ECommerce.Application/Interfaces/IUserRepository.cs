using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByTcknAsync(string tckn);
    }
}