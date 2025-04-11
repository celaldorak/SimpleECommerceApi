namespace ECommerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Preparing"; // Preparing -> Completed
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}