using AhorraYa.Abstractions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AhorraYa.Entities
{
    public class Product : IEntidad
    {
        public Product()
        {
            PriceOfShops = new HashSet<PriceOfShop>();
        }
        public Product(string producName, int categoryId, int brandId, string barCode)
        {
            SetProductName(producName);
            SetBarCode(barCode);
            SetCategoryId(categoryId);
            SetBrandId(brandId);
        }
        #region Properties
        public int Id { get; set; }
        [StringLength(50)]
        public string? Name { get; private set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; private set; }
        [JsonIgnore]

        [ForeignKey(nameof(Brand))]
        public int BrandId { get; private set; }
        [JsonIgnore]

        [StringLength(13)]
        [RegularExpression(@"^\{13}$")]
        public decimal? BarCode { get; set; }
        #endregion

        #region Virtual
        public virtual Category? Category { get; set; }
        public virtual Brand? Brand { get; set; }

        [JsonIgnore]
        public virtual ICollection<PriceOfShop> PriceOfShops { get; set; }
        #endregion

        #region Getters and Setters
        public void SetProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentNullException("The product name cannot be empty");
            }
            Name = productName;
        }

        public void SetBarCode(string barCode)
        {
            if (string.IsNullOrEmpty(barCode))
            {
                throw new ArgumentNullException("The product barcode cannot be empty");
            }
            if (barCode.Length != 13)
            {
                throw new ArgumentNullException("The barcode must be 13 digits long)");
            }
            if (!barCode.All(char.IsDigit))
            {                
                throw new ArgumentNullException("The barcode can only contain numbers)");
            }
            if(decimal.TryParse(barCode, out decimal result))
            {
                BarCode = result;
            }
        }
        public void SetCategoryId(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentNullException("Enter a valid number (Id)");
            }
            CategoryId = categoryId;
        }

        public void SetBrandId(int brandId)
        {
            if (brandId <= 0)
            {
                throw new ArgumentNullException("Enter a valid number (Id)");
            }
            BrandId = brandId;
        }
        #endregion

        // override object.Equals
        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is Product product)) return false;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(product.Name))
                return false;

            //Comparación insensible a mayúsculas/ minúsculas
            bool sameProductName = string.Equals(Name.Trim(), product.Name.Trim(), StringComparison.OrdinalIgnoreCase);
            bool sameBrandId = this.BrandId == product.BrandId;
            bool sameCategoryId = this.CategoryId == product.CategoryId;


            return sameProductName && sameBrandId && sameCategoryId;

        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Name?.Trim().ToLowerInvariant(),
                BrandId,
                CategoryId);
        }
    }
}
