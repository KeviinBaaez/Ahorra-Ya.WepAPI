using AhorraYa.Entities.MicrosoftIdentity;

namespace AhorraYa.Application.Dtos.Login
{
    public class LoginUserResponseDto
    {
        public string Token { get; set; }
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public bool Login { get; set; }
        public string? Role { get; set; }
        public List<string> Errores { get; set; }
    }
}
