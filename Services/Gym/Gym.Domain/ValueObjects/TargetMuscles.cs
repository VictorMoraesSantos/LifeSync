using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class TargetMuscles : ValueObject
    {
        public MuscleGroup PrimaryMuscle { get; private set; }
        private readonly List<MuscleGroup> _secondaryMuscles;
        public IReadOnlyCollection<MuscleGroup> SecondaryMuscles => _secondaryMuscles.AsReadOnly();

        protected TargetMuscles()
        {
            _secondaryMuscles = new List<MuscleGroup>();
        }

        public TargetMuscles(MuscleGroup primaryMuscle, List<MuscleGroup> secondaryMuscles = null)
        {
            PrimaryMuscle = primaryMuscle;
            _secondaryMuscles = secondaryMuscles?.Where(m => m != primaryMuscle).ToList() ?? new List<MuscleGroup>();
        }

        public void AddSecondaryMuscle(MuscleGroup muscle)
        {
            if (muscle == PrimaryMuscle)
                throw new DomainException("Secondary muscle cannot be the same as primary muscle");

            if (!_secondaryMuscles.Contains(muscle))
                _secondaryMuscles.Add(muscle);
        }

        public void RemoveSecondaryMuscle(MuscleGroup muscle)
        {
            _secondaryMuscles.Remove(muscle);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PrimaryMuscle;
            foreach (var muscle in _secondaryMuscles.OrderBy(m => m))
                yield return muscle;
        }
    }
}
