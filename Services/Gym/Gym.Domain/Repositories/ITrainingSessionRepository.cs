using Core.Domain.Repositories;
using Gym.Domain.Entities;

namespace Gym.Domain.Repositories
{
    public interface ITrainingSessionRepository : IRepository<TrainingSession, int>
    {
    }
}
