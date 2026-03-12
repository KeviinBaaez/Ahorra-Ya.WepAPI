namespace AhorraYa.WebClient.ViewModels.PricesOfShops
{
    public class PriceOfShopListVm
    {
        public int Id { get; set; }
        public string Product { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Shop { get; set; } = null!;
        public decimal Price { get; set; }
        public string LastModification { get; set; } = null!;
    }
}
