﻿using Core.Domain.Events;

namespace EmailSender.Domain.Events
{
    public class EmailSentEvent : DomainEvent
    {
        public string Email { get; }
        public DateTime SentAt { get; }

        public EmailSentEvent(string email, DateTime sentAt)
        {
            Email = email;
            SentAt = sentAt;
        }
    }
}
