using AhorraYa.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AhorraYa.Entities
{
    public class MeasurementUnit : IEntidad
    {
        public MeasurementUnit()
        {
            
        }

        public MeasurementUnit(string abbreviation)
        {
            SetAbbreviation(abbreviation);
        }

        #region Properties
        public int Id { get; set; }
        [Range(0, 999999.99)]
        public decimal Amount { get; private set; }
        [StringLength(5)]
        public string Abbreviation { get; set; } = null!;
        #endregion


        #region Getters and Setters
        public void SetAbbreviation(string abbreviation)
        {
            if (string.IsNullOrEmpty(abbreviation))
            {
                throw new ArgumentNullException("The unit of measure cannot be empty");
            }
            Abbreviation = abbreviation;
        }
        #endregion

        // override object.Equals
        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is MeasurementUnit measurement)) return false;

            if (string.IsNullOrWhiteSpace(Abbreviation) || string.IsNullOrWhiteSpace(measurement.Abbreviation))
                return false;

            // Comparación insensible a mayúsculas/minúsculas
            return string.Equals(this.Abbreviation.Trim(), measurement.Abbreviation.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Abbreviation?.Trim().ToLowerInvariant().GetHashCode() ?? 0;
        }
    }
}
