using Core.Domain.Entities;

namespace Nutrition.Domain.Entities
{
    public class LiquidType : BaseEntity<int>
    {
        public string Name { get; private set; }

        public LiquidType(string name)
        {
            if(!string.IsNullOrEmpty(name)) 
                Name = name;
        }
    }
}
