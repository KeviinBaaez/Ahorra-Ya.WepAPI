using AhorraYa.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Entities
{
    public class Location : IEntidad
    {

        public Location(string address, int number, int? floor , int cityId)
        {
            SetAddress(address);
            SetNumber(number);
            Floor = floor;
            CityId = cityId;
        }
        #region Properties
        public int Id { get; set; }

        [StringLength(60)]
        public string Address { get; private set; } = null!;
        public int Number { get; private set; }
        public int? Floor { get; set; }
        public int CityId { get; set; }
        #endregion

        #region Virtual
        public virtual City? City { get; set; }
        #endregion

        #region Getters and Setters
        public void SetAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("The address cannot be empty");
            }
            Address = address;
        }

        public void SetNumber(int number)
        {
            if(number < 0)
            {
                throw new ArgumentNullException("check that the height is a valid number");
            }
            Number = number;
        }

        public string GetFullAddress()
        {
            return $"{Address} {Number}, Floor: {Floor ?? 0}";
        }
        #endregion

        // override object.Equals
        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is Location location)) return false;

            if (string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(location.Address))
                return false;

            // Comparación insensible a mayúsculas/minúsculas
            bool sameAddress = string.Equals(Address.Trim(), location.Address.Trim(), StringComparison.OrdinalIgnoreCase);
            bool sameNumber = this.Number == location.Number;
            bool sameFloor = this.Floor == location.Floor;

            return sameAddress && sameNumber && sameFloor;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Address?.Trim().ToLowerInvariant(),
                Number, Floor);
        }
    }
}
