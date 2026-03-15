using Gym.Domain.Entities;

namespace Gym.UnitTests.Helpers.Builders;

public class TrainingSessionBuilder
{
    private int _userId = 1;
    private int _routineId = 1;
    private DateTime _startTime = DateTime.UtcNow.AddHours(-1);
    private DateTime? _endTime = null;

    public TrainingSessionBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public TrainingSessionBuilder WithRoutineId(int routineId)
    {
        _routineId = routineId;
        return this;
    }

    public TrainingSessionBuilder WithStartTime(DateTime startTime)
    {
        _startTime = startTime;
        return this;
    }

    public TrainingSessionBuilder WithEndTime(DateTime? endTime)
    {
        _endTime = endTime;
        return this;
    }

    public TrainingSession Build()
    {
        return new TrainingSession(_userId, _routineId, _startTime, _endTime);
    }

    public static TrainingSessionBuilder Default() => new();
}
