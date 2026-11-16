using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace VynilServer.Infrastructure.Services.EmailService;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    
    public EmailSender(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(new MailAddress(toEmail));

        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        try
        {
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($":Fail: Failed to send email to {toEmail}\n{ex.Message}");
            throw;
        }
    }

    public async Task SendRegistrationCodeAsync(string toEmail, string code)
    {
        var subject = $"Код подтверждения {_settings.FromName}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #333;'>Подтверждение регистрации</h2>
            <p>Ваш код подтверждения:</p>
            
            <div style='
                background-color: #f4f4f4; 
                border: 1px solid #ddd; 
                border-radius: 8px; 
                padding: 15px; 
                text-align: center; 
                font-size: 28px; 
                font-weight: bold; 
                letter-spacing: 4px; 
                color: #2c3e50;
                margin: 20px 0;'>
                {code}
            </div>
            
            <p style='color: #666; font-size: 14px;'>
                Код действителен в течение 10 минут.<br>
                Если вы не запрашивали регистрацию, просто проигнорируйте это письмо.
            </p>
        </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body);
    }
}