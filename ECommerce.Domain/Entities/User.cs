namespace ECommerce.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string TCKN { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public string Role { get; set; } = "User"; // "Admin" or "User"
        public ICollection<Order> Orders { get; set; }
    }
}