using Gym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastructure.Persistence.Configuration
{
    internal class RoutineExerciseConfiguration : IEntityTypeConfiguration<RoutineExercise>
    {
        public void Configure(EntityTypeBuilder<RoutineExercise> builder)
        {
            builder.OwnsOne(re => re.Sets);

            builder.OwnsOne(re => re.Repetitions);

            builder.OwnsOne(re => re.RestBetweenSets);

            builder.OwnsOne(re => re.RecommendedWeight);
        }
    }
}
