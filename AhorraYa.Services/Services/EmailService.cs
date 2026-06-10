using AhorraYa.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace AhorraYa.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string code)
        {
            var message = new MimeMessage();

            //Remitente (Mi App)
            message.From.Add(new MailboxAddress("AhorraYa", _configuration["EmailSettings:From"]));

            //Destinario (Usuario)
            message.To.Add(new MailboxAddress("", email));

            message.Subject = "Confirmá tu cuenta en AhorraYa";

            // Cuerpo del mensaje en HTML bien presentable con Bootstrap
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='font-family: sans-serif; max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #e9ecef; rounded-top: 8px;'>
                        <h2 style='color: #0d6efd; text-align: center;'>¡Bienvenido a AhorraYa!</h2>
                        <p style='color: #495057;'>Gracias por registrarte. Para completar la activación de tu cuenta e iniciar sesión, ingresá el siguiente código de verificación en la plataforma:</p>
                        <div style='background-color: #f8f9fa; padding: 15px; text-align: center; margin: 20px 0; border-radius: 8px;'>
                            <h1 style='letter-spacing: 6px; color: #198754; margin: 0; font-size: 32px;'>{code}</h1>
                        </div>
                        <p style='color: #6c757d; font-size: 12px; text-align: center;'>Este código es de un solo uso y vencerá pronto.</p>
                    </div>"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {

                //Conexión segura al servidor  SMTP de gmail (Puerto 587)
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                // Autenticación con tus credenciales seguras
                await client.AuthenticateAsync(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
