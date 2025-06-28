using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class SetExecution : ValueObject
    {
        public int SetNumber { get; private set; }
        public int Repetitions { get; private set; }
        public Weight Weight { get; private set; }
        public bool WasCompleted { get; private set; }

        protected SetExecution() { }

        public SetExecution(int setNumber, int repetitions, Weight weight, bool wasCompleted = true)
        {
            if (setNumber <= 0)
                throw new DomainException("Set number must be greater than zero");

            if (repetitions < 0)
                throw new DomainException("Repetitions cannot be negative");

            SetNumber = setNumber;
            Repetitions = repetitions;
            Weight = weight;
            WasCompleted = wasCompleted;
        }

        public void MarkAsCompleted()
        {
            WasCompleted = true;
        }

        public void MarkAsIncomplete()
        {
            WasCompleted = false;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SetNumber;
            yield return Repetitions;
            yield return Weight;
            yield return WasCompleted;
        }
    }
}
