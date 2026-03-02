using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Cities
{
    public class CityListVm
    {
        public int Id { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; } = null!;
        [DisplayName("Province")]
        public string ProvinceName { get; set; } = null!;
        [DisplayName("Country")]
        public string CountryName { get; set; } = null!;

    }
}
