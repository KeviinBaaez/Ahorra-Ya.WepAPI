namespace AhorraYa.Application.Dtos.City
{
    public class CityResponseDto
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
    }
}
