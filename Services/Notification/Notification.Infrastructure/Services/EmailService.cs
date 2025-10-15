using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using EmailSender.Domain.Entities;
using EmailSender.Infrastructure.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Infrastructure.Persistence.Data;

namespace EmailSender.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _cfg;
        private readonly ApplicationDbContext _context;

        public EmailService(IOptions<SmtpSettings> options, ApplicationDbContext context)
        {
            _cfg = options.Value;
            _context = context;
        }

        public Task<int> CreateEmail(EmailMessageDTO dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEmail(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailMessageDTO>> GetAllEmailMessages(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<EmailMessageDTO> GetEmailById(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(EmailMessageDTO dto, CancellationToken cancellationToken)
        {
            var mail = new EmailMessage("no-reply@yourdomain.com", dto.To, dto.Subject, dto.Body);
            var message = new MimeMessage();
            var from = string.IsNullOrWhiteSpace(_cfg.From) ? "no-reply@test.local" : _cfg.From;

            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(dto.To));
            message.Subject = dto.Subject;
            message.Body = new TextPart("plain") { Text = dto.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.None, cancellationToken);
            if (!string.IsNullOrWhiteSpace(_cfg.User))
                await client.AuthenticateAsync(_cfg.User, _cfg.Password, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        public Task<bool> UpdateEmail(EmailMessageDTO dt, CancellationToken cancellationTokeno)
        {
            throw new NotImplementedException();
        }
    }
}
