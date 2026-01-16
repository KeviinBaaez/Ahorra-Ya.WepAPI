using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Brands
{
    public class BrandListVm
    {
        public int Id { get; set; }
        [DisplayName("Brand Name")]
        public string BrandName { get; set; } = null!;
    }
}
