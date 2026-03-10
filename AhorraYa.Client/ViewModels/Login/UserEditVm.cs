using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AhorraYa.WebClient.ViewModels.Login
{
    public class UserEditVm
    {
        [DisplayName("Email")]
        public string? Email { get; set; }
        [DisplayName("UserName")]
        public string? UserName { get; set; }
        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
    }
}
