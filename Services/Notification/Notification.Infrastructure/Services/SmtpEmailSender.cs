using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using EmailSender.Domain.Entities;
using EmailSender.Infrastructure.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSender.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _cfg;

        public SmtpEmailSender(IOptions<SmtpSettings> options)
        {
            _cfg = options.Value;
        }

        public async Task SendEmailAsync(EmailMessageDTO dto)
        {
            var mail = new EmailMessage("no-reply@yourdomain.com", dto.To, dto.Subject, dto.Body);
            var message = new MimeMessage();
            var from = string.IsNullOrWhiteSpace(_cfg.From) ? "no-reply@test.local" : _cfg.From;

            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(dto.To));
            message.Subject = dto.Subject;
            message.Body = new TextPart("plain") { Text = dto.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.None);
            if (!string.IsNullOrWhiteSpace(_cfg.User))
                await client.AuthenticateAsync(_cfg.User, _cfg.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
