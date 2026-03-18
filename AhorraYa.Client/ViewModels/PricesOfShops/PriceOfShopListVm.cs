using Microsoft.AspNetCore.Mvc.Rendering;

namespace AhorraYa.WebClient.ViewModels.PricesOfShops
{
    public class PriceOfShopListVm
    {
        public int Id { get; set; }
        public string Product { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Shop { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ProductImage { get; set; }
        public string ShopName { get; set; } = null!;
        public string LastModification { get; set; } = null!;
        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Shops { get; set; }
    }
}
