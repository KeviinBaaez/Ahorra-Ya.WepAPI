using AhorraYa.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AhorraYa.Entities
{
    public class City : IEntidad
    {
        public City()
        {
            Locations = new HashSet<Location>();
        }

        public City(string city, int provinceId)
        {
            SetCity(city);
            SetProvinceId(provinceId);
        }
        #region Properties 
        public int Id { get; set; }
        [StringLength(50)]
        public string CityName { get; private set; }

        [ForeignKey(nameof(Province))]
        public int ProvinceId { get; private set; }
        #endregion

        #region Virtual
        public virtual Province? Province { get; set; }

        [JsonIgnore]
        public virtual ICollection<Location> Locations { get; set; }
        #endregion
        #region Getters and Setters

        public void SetCity(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                throw new ArgumentNullException("The City name cannot be empty");
            }
            CityName = city;
        }

        public void SetProvinceId(int provinceId)
        {
            if (provinceId <= 0)
            {
                throw new ArgumentNullException("Enter a valid number (Id)");
            }
            ProvinceId = provinceId;
        }
        #endregion
    }
}
