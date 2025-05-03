namespace Users.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string token, string subject = null, string body = null);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailHtmlAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentsAsync(string to, string subject, string body, bool isHtml = false, IEnumerable<(string FileName, byte[] Content)> attachments = null);
    }
}
