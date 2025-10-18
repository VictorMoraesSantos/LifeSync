namespace Users.Application.DTOs.Email
{
    public record EmailAttachmentDTO(string FileName, byte[] Content);

    public record EmailMessageDTO(
        string To,
        string? Subject = null,
        string? Body = null,
        bool IsHtml = true,
        IEnumerable<EmailAttachmentDTO>? Attachments = null,
        string? Token = null,
        string? CallbackUrl = null
    );
}