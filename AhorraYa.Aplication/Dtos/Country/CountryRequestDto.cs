using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.Country
{
    public class CountryRequestDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string CountryName { get; set; } = null!;
    }
}
