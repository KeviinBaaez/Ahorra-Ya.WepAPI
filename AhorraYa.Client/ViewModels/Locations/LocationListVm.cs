using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Locations
{
    public class LocationListVm
    {
        public int Id { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; } = null!;
        public int Number { get; set; }
        public int? Floor { get; set; }
        public string CityName { get; set; } = null!;
    }
}
