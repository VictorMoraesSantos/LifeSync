using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Financial.Domain.Errors;

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
            if (userId <= 0)
                throw new DomainException(CategoryErrors.InvalidUserId);
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(CategoryErrors.InvalidName);

            UserId = userId;
            Name = name;
            Description = description;
        }

        public void Update(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(CategoryErrors.InvalidName);

            Name = name;
            Description = description;
            MarkAsUpdated();
        }
    }
}
