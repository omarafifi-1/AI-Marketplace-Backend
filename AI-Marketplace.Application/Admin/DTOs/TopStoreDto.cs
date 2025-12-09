namespace AI_Marketplace.Application.Admin.DTOs
{
    public class TopStoreDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}