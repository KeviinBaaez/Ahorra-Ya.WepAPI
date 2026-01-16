using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.MeasurementUnit
{
    public class MeasurementUnitRequestDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; } 
        [StringLength(5)]
        public string Abbreviation { get; set; } = null!;
    }
}
