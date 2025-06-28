using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class SetRepetition : ValueObject
    {
        public int Sets { get; private set; }
        public int Repetitions { get; private set; }
        public Weight Weight { get; private set; }
        public Duration RestTime { get; private set; }

        protected SetRepetition() { } // For ORM

        public SetRepetition(int sets, int repetitions, Weight weight = null, Duration restTime = null)
        {
            if (sets <= 0)
                throw new DomainException("Number of sets must be greater than zero");

            if (repetitions <= 0)
                throw new DomainException("Number of repetitions must be greater than zero");

            Sets = sets;
            Repetitions = repetitions;
            Weight = weight;
            RestTime = restTime;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Sets;
            yield return Repetitions;
            yield return Weight;
            yield return RestTime;
        }
    }
}
