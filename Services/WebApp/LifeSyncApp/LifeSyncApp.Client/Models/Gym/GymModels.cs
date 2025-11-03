namespace LifeSyncApp.Client.Models.Gym
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MuscleGroup { get; set; }
        public int ExerciseType { get; set; }
        public int? EquipmentType { get; set; }
    }

    public class CreateExerciseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MuscleGroup { get; set; }
        public int ExerciseType { get; set; }
        public int? EquipmentType { get; set; }
    }

    public class UpdateExerciseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MuscleGroup { get; set; }
        public int ExerciseType { get; set; }
        public int? EquipmentType { get; set; }
    }

    public class RoutineDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateRoutineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoutineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TrainingSessionDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int RoutineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateTrainingSessionRequest
    {
        public int UserId { get; set; }
        public int RoutineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateTrainingSessionRequest
    {
        public int RoutineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
