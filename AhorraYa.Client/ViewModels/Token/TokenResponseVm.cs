namespace AhorraYa.WebClient.ViewModels.Token
{
    public class TokenResponseVm
    {
        public string Token { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public bool Login { get; set; }
        public string Role { get; set; }
        public List<string> Errores { get; set; }
    }
}
