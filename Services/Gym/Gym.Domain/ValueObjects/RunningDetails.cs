using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class RunningDetails : ValueObject
    {
        public RunningType Type { get; private set; }
        public bool IsOutdoor { get; private set; }
        public TerrainType PreferredTerrain { get; private set; }

        protected RunningDetails() { }

        public RunningDetails(RunningType type, bool isOutdoor, TerrainType preferredTerrain)
        {
            Type = type;
            IsOutdoor = isOutdoor;
            PreferredTerrain = preferredTerrain;

            ValidateTerrain();
        }

        private void ValidateTerrain()
        {
            if (!IsOutdoor && PreferredTerrain != TerrainType.Treadmill)
                throw new DomainException("Indoor running must use treadmill terrain");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return IsOutdoor;
            yield return PreferredTerrain;
        }
    }
}
