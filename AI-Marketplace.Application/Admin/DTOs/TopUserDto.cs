namespace AI_Marketplace.Application.Admin.DTOs
{
    public class TopUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal TotalSpending { get; set; }
        public int TotalOrders { get; set; }
    }
}