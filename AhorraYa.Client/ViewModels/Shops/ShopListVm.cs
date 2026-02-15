using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Shops
{
    public class ShopListVm
    {
        public int Id { get; set; }
        [DisplayName("ShopName")]
        public string ShopName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;

    }
}
