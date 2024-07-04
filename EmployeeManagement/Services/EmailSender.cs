using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mrgxwi2003@gmail.com", "pepe17102003")
            };

            return client.SendMailAsync(
                new MailMessage(from: "mrgxwi2003@gmail.com",
                                to: email,
                                subject,
                                message));
        }
    }
}
