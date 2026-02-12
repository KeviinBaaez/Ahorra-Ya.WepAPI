using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.Province
{
    public class ProvinceRequestDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string ProvinceName { get; set; } = null!;
        [StringLength(10)]
        public string Code { get; set; } = null!;
        public int CountryId { get; set; }
    }
}
