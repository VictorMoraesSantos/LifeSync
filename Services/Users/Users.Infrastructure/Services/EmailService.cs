using BuildingBlocks.Results;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Users.Application.DTOs.Email;
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
                Credentials = string.IsNullOrEmpty(_smtpSettings.User) ? null : new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };
        }

        public async Task<Result> SendConfirmationEmailAsync(EmailMessageDTO message)
        {
            try
            {
                var subject = message.Subject ?? "Confirmação de E-mail";

                var link = message.CallbackUrl ?? $"https://seusite.com/confirm-email?email={WebUtility.UrlEncode(message.To)}&token={WebUtility.UrlEncode(message.Token ?? string.Empty)}";

                var body = message.Body ?? $@"
                    <p>Olá,</p>
                    <p>Por favor, confirme seu e-mail clicando no link abaixo:</p>
                    <p><a href='{link}'>Confirmar E-mail</a></p>
                    <p>Se você não solicitou este e-mail, ignore-o.</p>";

                var dto = message with { Subject = subject, Body = body, IsHtml = true };
                return await SendEmailAsync(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Problem($"Failed to send confirmation email: {ex.Message}"));
            }
        }

        public async Task<Result> SendForgotPasswordEmailAsync(EmailMessageDTO message)
        {
            try
            {
                var subject = message.Subject ?? "Redefinição de Senha";
                var link = message.CallbackUrl ?? $"https://seusite.com/reset-password?email={WebUtility.UrlEncode(message.To)}&token={WebUtility.UrlEncode(message.Token ?? string.Empty)}";
                var body = message.Body ?? $@"
                    <p>Olá,</p>
                    <p>Você solicitou a redefinição de sua senha. Clique no link abaixo para criar uma nova senha:</p>
                    <p><a href='{link}'>Redefinir Senha</a></p>
                    <p>token: {WebUtility.HtmlEncode(message.Token ?? string.Empty)}</p>
                    <p>Se você não solicitou essa alteração, ignore este e-mail.</p>";
                var dto = message with { Subject = subject, Body = body, IsHtml = true };
                return await SendEmailAsync(dto);
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Problem($"Failed to send forgot-password email: {ex.Message}"));
            }
        }

        public async Task<Result> SendEmailAsync(EmailMessageDTO message)
        {
            try
            {
                using var mailMessage = new MailMessage(_fromAddress, message.To, message.Subject ?? string.Empty, message.Body ?? string.Empty)
                {
                    IsBodyHtml = message.IsHtml
                };

                if (message.Attachments != null)
                {
                    foreach (var att in message.Attachments)
                    {
                        var stream = new MemoryStream(att.Content);
                        var attachment = new Attachment(stream, att.FileName);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                await _smtpClient.SendMailAsync(mailMessage);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Problem($"Failed to send email: {ex.Message}"));
            }
        }

        public async Task<Result> SendEmailWithAttachmentsAsync(EmailMessageDTO message)
        {
            // Alias to SendEmailAsync since attachments are already included in the DTO
            return await SendEmailAsync(message);
        }
    }
}