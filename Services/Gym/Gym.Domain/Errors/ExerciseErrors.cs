using BuildingBlocks.Results;

namespace Gym.Domain.Errors
{
    public static class ExerciseErrors
    {
        // Validation errors
        public static Error InvalidName =>
            Error.Failure("Exercise name cannot be null or empty");

        public static Error InvalidDescription =>
            Error.Failure("Exercise description cannot be null or empty");

        public static Error InvalidMuscleGroup =>
            Error.Failure("Invalid muscle group specified");

        public static Error InvalidExerciseType =>
            Error.Failure("Invalid exercise type specified");

        public static Error InvalidEquipmentType =>
            Error.Failure("Invalid equipment type specified");

        // Operation errors
        public static Error NotFound(int id) =>
            Error.NotFound($"Exercise with ID {id} not found");

        public static Error NotFound() =>
            Error.NotFound($"Exercises not found");

        public static Error CreateError =>
            Error.Problem("Error creating exercise");

        public static Error UpdateError =>
            Error.Problem("Error updating exercise");

        public static Error DeleteError =>
            Error.Problem("Error deleting exercise");

        // Relationship errors
        public static Error InUseError =>
            Error.Conflict("Cannot delete exercise as it is in use");
    }
}