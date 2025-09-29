using EmailSender.Application.DTO;

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
