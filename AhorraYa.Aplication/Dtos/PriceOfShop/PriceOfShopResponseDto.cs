namespace AhorraYa.Application.Dtos.PriceOfShop
{
    public class PriceOfShopResponseDto
    {
        public int Id { get; set; }
        public string Product { get; set; } = null!;
        public string? ProductImage { get; set; }
        public string Brand { get; set; } = null!;
        public string Shop { get; set; } = null!;
        public string ShopName { get; set; } = null!;
        public decimal Price { get; set; }
        public string LastModification { get; set; } = null!;
    }
}
