namespace AhorraYa.Abstractions
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string code);
    }
}
