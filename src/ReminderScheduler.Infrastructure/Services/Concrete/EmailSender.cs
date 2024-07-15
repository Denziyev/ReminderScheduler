using Microsoft.Extensions.Configuration;
using ReminderScheduler.Application.Services.Abstract;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ReminderScheduler.Infrastructure.Services.Concrete
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string content)
        {
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("ilkindenziyev@gmail.com");
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body =content;
            mm.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("ilkindenziyev@gmail.com", "casfupqfywrlxxmx");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
           await smtp.SendMailAsync(mm);
        }
    }
}
