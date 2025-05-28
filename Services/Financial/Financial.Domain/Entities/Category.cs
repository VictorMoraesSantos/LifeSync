using Core.Domain.Entities;

namespace Financial.Domain.Entities
{
    public class Category : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        // For EF Core
        private Category() { }

        public Category(int userId, string name, string? description = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            UserId = userId;
            Name = name;
            Description = description;
        }

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            Name = name;
            Description = description;
            MarkAsUpdated();
        }
    }
}
