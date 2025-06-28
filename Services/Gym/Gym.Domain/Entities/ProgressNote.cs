using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Gym.Domain.Entities
{
    public class ProgressNote : BaseEntity<int>
    {
        public string Content { get; private set; }
        public string Category { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected ProgressNote() { }

        public ProgressNote(string content, string category = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Note content cannot be empty");

            Content = content;
            Category = category ?? "";
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string content, string category = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Note content cannot be empty");

            Content = content;

            if (category != null)
                Category = category;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
