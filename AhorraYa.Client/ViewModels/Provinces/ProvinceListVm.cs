using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Provinces
{
    public class ProvinceListVm
    {
        public int Id { get; set; }
        [DisplayName("Privince Name")]
        public string ProvinceName { get; set; } = null!;
        public string Code { get; set; }
        public string CountryName { get; set; }
    }
}
