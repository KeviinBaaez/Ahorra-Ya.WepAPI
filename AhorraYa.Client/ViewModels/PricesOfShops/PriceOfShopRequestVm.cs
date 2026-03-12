using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AhorraYa.WebClient.ViewModels.PricesOfShops
{
    public class PriceOfShopRequestVm
    {
        public int Id { get; set; }
        [DisplayName("Product")]
        public int ProductId { get; set; }
        [DisplayName("Shop")]
        public int ShopId { get; set; }
        [DisplayName("Price")]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTime RegistrationDate { get; set; }
    }
}
