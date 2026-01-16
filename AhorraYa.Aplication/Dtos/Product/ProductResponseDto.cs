namespace AhorraYa.Application.Dtos.Product
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal BarCode { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;

    }
}
