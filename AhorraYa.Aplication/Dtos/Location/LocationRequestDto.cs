using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.Location
{
    public class LocationRequestDto
    {
        public int Id { get; set; }

        [StringLength(60)]
        public string Address { get; set; } = null!;
        public int Number { get; set; }
        public int? Floor { get; set; }
        public int CityId { get; set; }
    }
}
