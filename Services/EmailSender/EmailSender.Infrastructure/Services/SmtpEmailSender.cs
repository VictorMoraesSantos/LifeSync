using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using System.Net.Mail;

namespace EmailSender.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public SmtpEmailSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(EmailMessageDTO dto)
        {
            var mail = new MailMessage("no-reply@yourdomain.com", dto.To, dto.Subject, dto.Body);
            await _smtpClient.SendMailAsync(mail);
        }
    }
}
