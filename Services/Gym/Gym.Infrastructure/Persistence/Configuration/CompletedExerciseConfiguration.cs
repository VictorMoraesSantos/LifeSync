using Gym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastructure.Persistence.Configuration
{
    public class CompletedExerciseConfiguration : IEntityTypeConfiguration<CompletedExercise>
    {
        public void Configure(EntityTypeBuilder<CompletedExercise> builder)
        {
            builder.OwnsOne(ce => ce.SetsCompleted);

            builder.OwnsOne(ce => ce.RepetitionsCompleted);

            builder.OwnsOne(ce => ce.SetsCompleted);

            builder.OwnsOne(ce => ce.WeightUsed);
        }
    }
}
