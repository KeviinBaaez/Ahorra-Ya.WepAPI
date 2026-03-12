using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Product
{
    public class ProductListVm
    {
        public int Id { get; set; }
        [DisplayName("Product Name")]
        public string Name { get; set; } = null!;
        public decimal BarCode { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }

        public string? Image { get; set; }
    }
}
