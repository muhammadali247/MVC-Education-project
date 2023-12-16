using EduHome.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace EduHome.Services.Implementations;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        //string fromEmail = "magoo.everywhere123@gmail.com";
        //string password = "u e e z y g j a j o l j g f g i  ";
        //string smtpHost = "smtp.googlemail.com";
        //int smptport = 587;
        string fromEmail = _configuration["MailSettings:Mail"];
        string password = _configuration["MailSettings:Password"];
        string smtpHost = _configuration["MailSettings:Host"];
        int smptport = int.Parse(_configuration["MailSettings:Port"]);

        SmtpClient smtpClient = new SmtpClient(smtpHost, smptport)
        {
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true
        };

        MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
        {
            IsBodyHtml = true
        };

        smtpClient.Send(mailMessage);
    }
}
