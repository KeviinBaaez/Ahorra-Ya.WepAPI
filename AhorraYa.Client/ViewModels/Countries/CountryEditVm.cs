using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Countries
{
    public class CountryEditVm
    {
        public int Id { get; set; }
        [DisplayName("Country Name")]
        public string CountryName { get; set; } = null!;
    }
}
