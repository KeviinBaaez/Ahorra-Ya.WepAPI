namespace AhorraYa.Application.Dtos.MeasurementUnit
{
    public class MeasurementUnitResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Abbreviation { get; set; } = null!;
    }
}
