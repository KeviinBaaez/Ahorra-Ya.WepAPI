using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Provinces
{
    public class ProvinceEditVm
    {
        public int Id { get; set; }
        [DisplayName("Province Name")]
        public string ProvinceName { get; set; } = null!;
        [DisplayName("Code")]
        public string Code { get; set; } = null!;
        [DisplayName("CountryId")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem>? Countries { get; set; }
    }
}
