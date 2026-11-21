using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace VynilServer.Infrastructure.Services.EmailService;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;

    public EmailSender(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    private const string StyleText = "color:#4b5563; font-size:15px; line-height:1.5; margin:0 0 16px 0;";
    private const string StyleCode = "background-color:#f3f4f6; border:1px solid #d1d5db; border-radius:6px; padding:12px; text-align:center; font-size:24px; font-weight:800; letter-spacing:3px; color:#111827; margin:16px 0;";
    private const string StyleLink = "display:inline-block; background-color:#2563eb; color:#ffffff !important; text-decoration:none; font-weight:600; padding:10px 20px; border-radius:6px; text-align:center; margin:16px 0;";
    private const string StyleLabel = "font-size:11px; color:#6b7280; text-transform:uppercase; letter-spacing:0.5px; font-weight:600; margin:0 0 6px 0;";

    public async Task SendDynamicEmailAsync(string toEmail, string subject, IEnumerable<string> labels, IEnumerable<string> values)
    {
        var labelList = labels.ToList();
        var valueList = values.ToList();

        if (labelList.Count != valueList.Count)
            throw new ArgumentException("Количество заголовков и значений должно совпадать.");

        var contentBuilder = new StringBuilder();

        for (int i = 0; i < labelList.Count; i++)
        {
            string val = valueList[i];
            string appliedStyle = val.StartsWith("http://") || val.StartsWith("https://") 
                ? StyleLink 
                : (val.Length <= 10 && val.All(char.IsDigit) ? StyleCode : StyleText);

            contentBuilder.AppendLine($@"
            <div style='margin-bottom:20px; padding:16px; background-color:#f8fafc; border-radius:6px; border-left:4px solid #2563eb;'>
                <p style='{StyleLabel}'>{labelList[i]}</p>
                <div style='{appliedStyle}'>{val}</div>
            </div>");
        }

        await SendCustomEmailAsync(toEmail, subject, contentBuilder.ToString());
    }

    public async Task SendCustomEmailAsync(string toEmail, string subject, string customBodyHtml)
    {
        string finalBody = $@"
        <!DOCTYPE html>
        <html>
        <body style='margin:0; padding:0; background-color:#f9fafb; font-family:Arial, sans-serif;'>
            <table role='presentation' width='100%' style='background-color:#f9fafb; padding:40px 20px;'>
                <tr><td align='center'>
                    <table role='presentation' width='100%' style='max-width:500px; background-color:#ffffff; border-radius:8px; overflow:hidden;'>
                        <tr><td style='background-color:#2563eb; padding:20px; text-align:center;'>
                            <h1 style='margin:0; color:#ffffff; font-size:20px; font-weight:700;'>{_settings.FromName}</h1>
                        </td></tr>
                        <tr><td style='padding:30px;'>
                            {customBodyHtml}
                        </td></tr>
                        <tr><td style='background-color:#f3f4f6; padding:15px; text-align:center; border-top:1px solid #e5e7eb;'>
                            <p style='margin:0; color:#6b7280; font-size:12px;'>Если вы не запрашивали это действие, проигнорируйте это письмо.</p>
                        </td></tr>
                    </table>
                </td></tr>
            </table>
        </body>
        </html>";

        await SendRawEmailAsync(toEmail, subject, finalBody);
    }

    private async Task SendRawEmailAsync(string toEmail, string subject, string htmlBody)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(toEmail));

        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        await client.SendMailAsync(message);
    }
}