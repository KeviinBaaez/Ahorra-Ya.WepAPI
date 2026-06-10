namespace AhorraYa.Application.Dtos.Identity.User
{
    public class VerifyCodeRequestDto
    {
        public string UserName { get; set; }
        public string Code { get; set; }
    }
}
