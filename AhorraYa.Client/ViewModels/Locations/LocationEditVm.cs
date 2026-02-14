using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Locations
{
    public class LocationEditVm
    {
        public int Id { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; } = null!;
        [DisplayName("Number")]
        public int Number { get; set; }
        [DisplayName("Floor")]
        public int? Floor { get; set; }
        [DisplayName("CityId")]
        public int CityId{ get; set; }

        public IEnumerable<SelectListItem>? Cities { get; set; }
    }
}
