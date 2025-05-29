using Core.Domain.Entities;

namespace Financial.Domain.Entities
{
    public class Category : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private Category() { }

        public Category(int userId, string name, string? description = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            UserId = userId;
            Name = name;
            Description = description;
        }

        public void Update(string name, string? description)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description);

            Name = name;
            Description = description;
            MarkAsUpdated();
        }
    }
}
