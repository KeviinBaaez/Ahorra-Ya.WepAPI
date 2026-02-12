namespace AhorraYa.Application.Dtos.Province
{
    public class ProvinceResponseDto
    {
        public int Id { get; set; }
        public string ProvinceName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
