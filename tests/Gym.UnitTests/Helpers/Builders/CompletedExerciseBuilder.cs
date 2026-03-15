using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;

namespace Gym.UnitTests.Helpers.Builders;

public class CompletedExerciseBuilder
{
    private int _trainingSessionId = 1;
    private int _routineExerciseId = 1;
    private SetCount _setsCompleted = SetCount.Create(3);
    private RepetitionCount _repetitionsCompleted = RepetitionCount.Create(10);
    private Weight? _weightUsed = Weight.Create(60, MeasurementUnit.Kilogram);
    private string? _notes = null;

    public CompletedExerciseBuilder WithTrainingSessionId(int trainingSessionId)
    {
        _trainingSessionId = trainingSessionId;
        return this;
    }

    public CompletedExerciseBuilder WithRoutineExerciseId(int routineExerciseId)
    {
        _routineExerciseId = routineExerciseId;
        return this;
    }

    public CompletedExerciseBuilder WithSetsCompleted(SetCount setsCompleted)
    {
        _setsCompleted = setsCompleted;
        return this;
    }

    public CompletedExerciseBuilder WithRepetitionsCompleted(RepetitionCount repetitionsCompleted)
    {
        _repetitionsCompleted = repetitionsCompleted;
        return this;
    }

    public CompletedExerciseBuilder WithWeightUsed(Weight? weightUsed)
    {
        _weightUsed = weightUsed;
        return this;
    }

    public CompletedExerciseBuilder WithNotes(string? notes)
    {
        _notes = notes;
        return this;
    }

    public CompletedExercise Build()
    {
        return new CompletedExercise(
            _trainingSessionId,
            _routineExerciseId,
            _setsCompleted,
            _repetitionsCompleted,
            _weightUsed,
            _notes);
    }

    public static CompletedExerciseBuilder Default() => new();
}
