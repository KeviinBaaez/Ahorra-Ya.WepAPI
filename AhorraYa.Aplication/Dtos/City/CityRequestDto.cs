using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.City
{
    public class CityRequestDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string CityName { get; set; } = null!;
        public int ProvinceId { get; set; }
    }
}
