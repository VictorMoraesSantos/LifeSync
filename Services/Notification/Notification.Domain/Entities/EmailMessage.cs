using Core.Domain.Entities;

namespace EmailSender.Domain.Entities
{
    public class EmailMessage : BaseEntity<int>
    {
        public string From { get; set; }
        public string To { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }

        public EmailMessage(string from, string to, string subject, string body)
        {
            From = from;
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
