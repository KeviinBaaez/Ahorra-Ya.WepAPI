using AhorraYa.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AhorraYa.Entities
{
    public class City : IEntidad
    {
        public City()
        {
            Locations = new HashSet<Location>();
        }

        public City(string city)
        {
            SetCity(city);
        }
        #region Properties 
        public int Id { get; set; }
        [StringLength(50)]
        public string CityName { get; set; }
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
        #endregion
    }
}
