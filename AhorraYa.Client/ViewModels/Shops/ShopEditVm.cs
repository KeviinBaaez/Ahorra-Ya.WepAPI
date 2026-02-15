using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Shops
{
    public class ShopEditVm
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        public string ShopName { get; set; } = null!;
        [DisplayName("LocationId")]
        public int LocationId { get; set; }

        public IEnumerable<SelectListItem>? Locations { get; set; }
    }
}
