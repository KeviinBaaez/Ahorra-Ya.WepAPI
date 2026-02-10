using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AhorraYa.Entities
{
    public class Province
    {
        public Province()
        {
            Cities = new HashSet<City>();
        }

        public Province(string provincia, string code)
        {
            SetProvincia(provincia);
            SetCode(code);
        }
        #region Properties 
        public int Id { get; set; }
        [StringLength(50)]
        public string ProvinceName { get; set; }
        [StringLength(10)]
        public string Code { get; set; }
        #endregion

        #region Virtual
        public virtual Country? Country { get; set; }

        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; }
        #endregion
        #region Getters and Setters

        public void SetProvincia(string province)
        {
            if (string.IsNullOrEmpty(province))
            {
                throw new ArgumentNullException("The province name cannot be empty");
            }
            ProvinceName = province;
        }

        public void SetCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("The province code cannot be empty");
            }
            Code = code;
        }
        #endregion
    }
}
