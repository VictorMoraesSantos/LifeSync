using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

namespace Nutrition.Domain.Entities
{
    public class LiquidType : BaseEntity<int>
    {
        public string Name { get; private set; }

        public LiquidType(string name)
        {
            SetName(name);
        }

        public void Update(string name)
        {
            SetName(name);
            MarkAsUpdated();
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(LiquidTypeErrors.InvalidName);
            Name = name;
        }
    }
}
