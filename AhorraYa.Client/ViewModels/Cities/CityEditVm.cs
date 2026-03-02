using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Cities
{
    public class CityEditVm
    {
        public int Id { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; } = null!;
        [DisplayName("ProvinceId")]
        public int ProvinceId { get; set; }
        public IEnumerable<SelectListItem>? Provinces { get; set; }
    }
}
