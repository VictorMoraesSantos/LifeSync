namespace EmailSender.Application.DTO
{
    public record EmailMessageDTO(
        string To,
        string Subject,
        string Body);
}
