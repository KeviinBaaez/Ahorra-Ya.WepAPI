using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Countries
{
    public class CountryListVm
    {
        public int Id { get; set; }
        [DisplayName("Country")]
        public string CountryName { get; set; } = null!;
    }
}
