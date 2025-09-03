using EmailSender.Application.DTO;

namespace EmailSender.Application.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessageDTO dto);
    }
}
