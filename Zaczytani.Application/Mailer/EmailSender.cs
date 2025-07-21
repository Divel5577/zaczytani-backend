using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Zaczytani.Application.Configuration;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Application.Mailer;

public class EmailSender(IOptions<MailerSettings> mailerSettings, IConfiguration configuration) : IEmailSender<User>
{
    private readonly MailerSettings _settings = mailerSettings.Value;
    private readonly string _frontendUrl = configuration.GetSection("FrontendUrl").Value
        ?? throw new InvalidOperationException("Frontend URL is not configured. Please set 'FrontendUrl' in appsettings.json.");

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var subject = user.EmailConfirmed ? "Potwierdzenie zmiany adresu e-mail" : "Potwierdzenie rejestracji";

        var actionDescription = user.EmailConfirmed ? "zmianę adresu e-mail" : "rejestrację";
        var message = $@"
        <h1>Witaj {user.UserName}!</h1>
        <p>Aby potwierdzić {actionDescription}, kliknij w poniższy link:</p>
        <p><a href='{confirmationLink}'>Potwierdź {actionDescription}</a></p>";

        await SendEmailAsync(email, subject, message);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        var subject = "Resetowanie hasła";
        var message = $@"
        <h1>Resetowanie hasła</h1>
        <p>Cześć {user.UserName}, aby zresetować swoje hasło, kliknij w poniższy link:</p>
        <a href='{_frontendUrl}/auth/resetPassword?resetCode={resetCode}&email={email}'>Zresetuj hasło</a>
        <p>Oto Twój kod resetujący hasło:</p>
        <h4>{resetCode}</h4>";

        await SendEmailAsync(email, subject, message);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var subject = "Resetowanie hasła";
        var message = $@"
        <h1>Resetowanie hasła</h1>
        <p>Cześć {user.UserName}, aby zresetować swoje hasło, kliknij w poniższy link:</p>
        <a href='{resetLink}'>Zresetuj hasło</a>";

        await SendEmailAsync(email, subject, message);
    }

    private async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.User),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        using (var client = new SmtpClient(_settings.Host, _settings.Port))
        {
            client.Credentials = new NetworkCredential(_settings.User, _settings.Password);
            client.EnableSsl = true;

            await client.SendMailAsync(mailMessage);
        }

    }
}