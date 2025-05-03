using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Users.Application.Interfaces;
using Users.Infrastructure.Smtp;

namespace Users.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
            _fromAddress = _smtpSettings.From;

            _smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };
        }

        public async Task SendConfirmationEmailAsync(string email, string token, string subject = null, string body = null)
        {
            subject ??= "Confirmação de E-mail";
            string confirmationLink = $"https://seusite.com/confirm-email?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}";

            body ??= $@"
                <p>Olá,</p>
                <p>Por favor, confirme seu e-mail clicando no link abaixo:</p>
                <p><a href='{confirmationLink}'>Confirmar E-mail</a></p>
                <p>Se você não solicitou este e-mail, ignore-o.</p>";

            await SendEmailHtmlAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromAddress, to, subject, body)
            {
                IsBodyHtml = false
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailHtmlAsync(string to, string subject, string htmlBody)
        {
            var mailMessage = new MailMessage(_fromAddress, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailWithAttachmentsAsync(string to, string subject, string body, bool isHtml = false, IEnumerable<(string FileName, byte[] Content)> attachments = null)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            if (attachments != null)
            {
                foreach (var (FileName, Content) in attachments)
                {
                    var stream = new MemoryStream(Content);
                    var attachment = new Attachment(stream, FileName);
                    mailMessage.Attachments.Add(attachment);
                }
            }

            await _smtpClient.SendMailAsync(mailMessage);

            foreach (var attachment in mailMessage.Attachments)
            {
                attachment.ContentStream.Dispose();
            }
        }
    }
}