namespace AI_Marketplace.Application.Admin.DTOs
{
    public class TopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}