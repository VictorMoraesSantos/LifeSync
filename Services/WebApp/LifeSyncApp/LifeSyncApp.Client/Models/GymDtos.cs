namespace LifeSyncApp.Client.Models.Gym;

public enum MuscleGroup
{
    Chest,
    Back,
    Shoulders,
    Biceps,
    Triceps,
    Forearms,
    Abs,
    Quadriceps,
    Hamstrings,
    Calves,
    Glutes,
    LowerBack,
    Traps,
    Neck,
    FullBody,
    Core
}

public enum ExerciseType
{
    Strength,
    Hypertrophy,
    Endurance,
    Power,
    Flexibility,
    Cardio,
    HIIT,
    Recovery
}

public enum EquipmentType
{
    Barbell,
    Dumbbell,
    Machine,
    Cable,
    Bodyweight,
    ResistanceBand,
    Kettlebell,
    MedicineBall,
    FoamRoller,
    BattleRope,
    PullUpBar,
    Bench,
    Other
}

// Read DTOs
public record ExerciseDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    string Description,
    MuscleGroup MuscleGroup,
    ExerciseType Type,
    EquipmentType? EquipmentType);

public record RoutineDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    string Description);

public record TrainingSessionDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int UserId,
    int RoutineId,
    DateTime StartTime,
    DateTime? EndTime,
    string? Notes);

// Commands
public record CreateExerciseCommand(
    string Name,
    string Description,
    MuscleGroup MuscleGroup,
    ExerciseType ExerciseType,
    EquipmentType? EquipmentType);

public record UpdateExerciseCommand(
    int Id,
    string Name,
    string Description,
    MuscleGroup MuscleGroup,
    ExerciseType ExerciseType,
    EquipmentType? EquipmentType);

public record CreateRoutineCommand(string Name, string Description);
public record UpdateRoutineCommand(int Id, string Name, string Description);

public record CreateTrainingSessionCommand(int UserId, int RoutineId, DateTime StartTime, DateTime EndTime);
public record UpdateTrainingSessionCommand(int Id, int RoutineId, DateTime StartTime, DateTime EndTime, string Notes);
