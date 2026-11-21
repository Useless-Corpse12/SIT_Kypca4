namespace VynilServer.Infrastructure.Services.EmailService;



public interface IEmailSender
{
    Task SendDynamicEmailAsync(string toEmail, string subject, IEnumerable<string> labels, IEnumerable<string> values); 
    Task SendCustomEmailAsync(string toEmail, string subject, string customBodyHtml);
}