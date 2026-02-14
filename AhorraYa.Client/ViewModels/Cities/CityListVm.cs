using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Cities
{
    public class CityListVm
    {
        public int Id { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; } = null!;
        public string ProvinceName { get; set; } = null!;

    }
}
