using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Product
{
    public class ProductEditVm
    {
        public int Id { get; set; }
        [DisplayName("Product Name")]
        public string Name { get; set; } = null!;
        [DisplayName("Bar Code")]
        public decimal BarCode { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("BrandId")]
        public int BrandId { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Brands { get; set; }
    }
}
