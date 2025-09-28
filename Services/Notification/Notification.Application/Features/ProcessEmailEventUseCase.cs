using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;

namespace EmailSender.Application.Features
{
    public class ProcessEmailEventUseCase
    {
        private readonly IEmailService _emailSender;

        public ProcessEmailEventUseCase(IEmailService emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task HandleAsync(string eventType, string eventData)
        {
            var email = eventType switch
            {
                "UserRegistered" => new EmailMessageDTO(
                    To: ExtractEmail(eventData),
                    Subject: "Welcome!",
                    Body: "Thanks for registering."),
                "OrderPlaced" => new EmailMessageDTO(
                    To: ExtractEmail(eventData),
                    Subject: "Order Confirmation",
                    Body: "Your order has been placed."),
                _ => null
            };

            if (email != null)
            {
                await _emailSender.SendEmailAsync(email);
            }
        }

        private string ExtractEmail(string eventData)
        {
            // Aqui você pode desserializar o eventData e extrair o email
            // Exemplo simplificado:
            return eventData;
        }
    }
}
