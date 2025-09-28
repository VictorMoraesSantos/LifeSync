using EmailSender.Application.DTO;
using EmailSender.Domain.Entities;

namespace EmailSender.Application.Contracts
{
    public interface IEmailService
    {
        Task<EmailMessageDTO> GetEmailById(int id);
        Task<IEnumerable<EmailMessageDTO>> GetAllEmailMessages();
        Task<int> CreateEmail(EmailMessageDTO dto);
        Task<bool> UpdateEmail(EmailMessageDTO dto);
        Task<bool> DeleteEmail(int id);
        Task SendEmailAsync(EmailMessageDTO dto);
    }
}
