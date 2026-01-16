using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Application.Dtos.Product
{
    public class ProductRequestDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public decimal BarCode { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
