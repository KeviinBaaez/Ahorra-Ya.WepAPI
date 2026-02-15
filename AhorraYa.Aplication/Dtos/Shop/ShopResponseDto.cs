namespace AhorraYa.Application.Dtos.Shop
{
    public class ShopResponseDto
    {
        public int Id { get; set; }
        public string ShopName { get; set; } = null!;
        public int LocationId { get; set; }
        public string? Address { get; set; }
        public string City { get; set; } = null!;
    }
}
