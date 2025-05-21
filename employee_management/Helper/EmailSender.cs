using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace employee_management.Helper
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Ethereal test credentials
            var host = "smtp.ethereal.email";
            var port = 587;
            var username = "mylene.nicolas@ethereal.email";
            var password = "aeDqDhMH1N5F8UaAxA";
            var fromEmail = "mylene.nicolas@ethereal.email";

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
