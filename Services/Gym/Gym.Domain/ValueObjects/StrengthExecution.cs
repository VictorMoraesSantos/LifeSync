using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class StrengthExecution : ValueObject
    {
        private readonly List<SetExecution> _sets;
        public IReadOnlyCollection<SetExecution> Sets => _sets.AsReadOnly();

        protected StrengthExecution()
        {
            _sets = new List<SetExecution>();
        }

        public StrengthExecution(List<SetExecution> sets)
        {
            if (sets == null || sets.Count == 0)
                throw new DomainException("At least one set must be provided");

            _sets = sets;
        }

        public void AddSet(SetExecution set)
        {
            if (set == null)
                throw new DomainException("Set cannot be null");

            _sets.Add(set);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var set in _sets)
                yield return set;
        }
    }
}
