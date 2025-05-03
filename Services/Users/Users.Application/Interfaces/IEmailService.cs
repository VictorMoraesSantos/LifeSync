namespace Users.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailHtmlAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentsAsync(string to, string subject, string body, bool isHtml = false, IEnumerable<(string FileName, byte[] Content)> attachments = null);
    }
}
