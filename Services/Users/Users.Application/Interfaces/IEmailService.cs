using BuildingBlocks.Results;
using Users.Application.DTOs.Email;

namespace Users.Application.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendConfirmationEmailAsync(EmailMessageDTO message);
        Task<Result> SendForgotPasswordEmailAsync(EmailMessageDTO message);
        Task<Result> SendEmailAsync(EmailMessageDTO message);
        Task<Result> SendEmailWithAttachmentsAsync(EmailMessageDTO message);
    }
}
