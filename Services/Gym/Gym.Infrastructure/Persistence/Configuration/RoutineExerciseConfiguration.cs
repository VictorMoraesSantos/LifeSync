using Gym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
