using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;
using Gym.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Services
{
    public class TrainingSessionService : ITrainingSessionService
    {
        private readonly ITrainingSessionRepository _trainingSessionRepository;

        public TrainingSessionService(ITrainingSessionRepository trainingSessionRepository)
        {
            _trainingSessionRepository = trainingSessionRepository;
        }

        public Task<Result<int>> CountAsync(Expression<Func<TrainingSessionDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> CreateAsync(CreateTrainingSessionDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTrainingSessionDTO> dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TrainingSessionDTO?>>> FindAsync(Expression<Func<TrainingSessionDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TrainingSessionDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TrainingSessionDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<(IEnumerable<TrainingSessionDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateAsync(UpdateTrainingSessionDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
