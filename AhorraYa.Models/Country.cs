using AhorraYa.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AhorraYa.Entities
{
    public class Country : IEntidad
    {
        public Country()
        {
            Provinces = new HashSet<Province>();
        }

        public Country(string country)
        {
            SetCountry(country);
        }
        #region Properties 
        public int Id { get; set; }
        [StringLength(50)]
        public string CountryName { get; set; }
        #endregion

        [JsonIgnore]
        public virtual ICollection<Province> Provinces { get; set; }
        #region Getters and Setters

        public void SetCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentNullException("The country name cannot be empty");
            }
            CountryName = country;
        }
        #endregion
    }
}
