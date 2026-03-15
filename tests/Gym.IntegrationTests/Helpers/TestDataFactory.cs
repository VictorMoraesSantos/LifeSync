using Bogus;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;

namespace Gym.IntegrationTests.Helpers
{
    public static class TestDataFactory
    {
        public static Exercise CreateExercise(
            string? name = null,
            MuscleGroup? muscleGroup = null,
            ExerciseType? type = null,
            EquipmentType? equipmentType = null)
        {
            var faker = new Faker();
            return new Exercise(
                name ?? faker.Lorem.Word() + " " + faker.Lorem.Word(),
                faker.Lorem.Sentence(),
                muscleGroup ?? faker.PickRandom<MuscleGroup>(),
                type ?? faker.PickRandom<ExerciseType>(),
                equipmentType ?? faker.PickRandom<EquipmentType>());
        }

        public static List<Exercise> CreateExercises(int count)
        {
            var exercises = new List<Exercise>();
            for (int i = 0; i < count; i++)
            {
                exercises.Add(CreateExercise());
            }
            return exercises;
        }

        public static Routine CreateRoutine(string? name = null)
        {
            var faker = new Faker();
            return new Routine(
                name ?? faker.Lorem.Word() + " Routine",
                faker.Lorem.Sentence());
        }

        public static List<Routine> CreateRoutines(int count)
        {
            var routines = new List<Routine>();
            for (int i = 0; i < count; i++)
            {
                routines.Add(CreateRoutine());
            }
            return routines;
        }

        public static RoutineExercise CreateRoutineExercise(
            int routineId,
            int exerciseId,
            int sets = 3,
            int reps = 12,
            int restSeconds = 60)
        {
            return new RoutineExercise(
                routineId,
                exerciseId,
                SetCount.Create(sets),
                RepetitionCount.Create(reps),
                RestTime.Create(restSeconds),
                Weight.Create(20, MeasurementUnit.Kilogram),
                "Test instructions");
        }

        public static TrainingSession CreateTrainingSession(
            int userId = 1,
            int routineId = 1,
            DateTime? startTime = null,
            DateTime? endTime = null)
        {
            var start = startTime ?? DateTime.UtcNow.AddHours(-1);
            return new TrainingSession(userId, routineId, start, endTime);
        }

        public static CompletedExercise CreateCompletedExercise(
            int trainingSessionId,
            int routineExerciseId,
            int sets = 3,
            int reps = 12)
        {
            return new CompletedExercise(
                trainingSessionId,
                routineExerciseId,
                SetCount.Create(sets),
                RepetitionCount.Create(reps),
                Weight.Create(20, MeasurementUnit.Kilogram),
                "Completed exercise notes");
        }
    }
}
