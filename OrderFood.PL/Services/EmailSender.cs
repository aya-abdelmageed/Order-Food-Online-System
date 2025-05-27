using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_configuration["Email:Smtp"])
        {
            Port = int.Parse(_configuration["Email:Port"]),
            Credentials = new NetworkCredential(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
            ),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:Username"], "OrderFood Team"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        return smtpClient.SendMailAsync(mailMessage);
    }
}
