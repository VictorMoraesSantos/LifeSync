using EmailSender.Application.DTO;

namespace EmailSender.Application.Contracts
{
    public interface IEmailService
    {
        Task<EmailMessageDTO> GetEmailById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EmailMessageDTO>> GetAllEmailMessages(CancellationToken cancellationToken);
        Task<int> CreateEmail(EmailMessageDTO dto, CancellationToken cancellationToken);
        Task<bool> UpdateEmail(EmailMessageDTO dto, CancellationToken cancellationToken);
        Task<bool> DeleteEmail(int id, CancellationToken cancellationToken);
        Task SendEmailAsync(EmailMessageDTO dto, CancellationToken cancellationToken);
    }
}
