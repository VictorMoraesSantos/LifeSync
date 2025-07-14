using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.RoutineExercise;
using Gym.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Services
{
    public class RoutineExerciseService : IRoutineExerciseService
    {
        private readonly IRoutineExerciseRepository _routineExerciseRepository;

        public RoutineExerciseService(IRoutineExerciseRepository routineExerciseRepository)
        {
            _routineExerciseRepository = routineExerciseRepository;
        }

        public Task<Result<int>> CountAsync(Expression<Func<RoutineExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> CreateAsync(CreateRoutineExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateRoutineExerciseDTO> dto, CancellationToken cancellationToken = default)
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

        public Task<Result<IEnumerable<RoutineExerciseDTO?>>> FindAsync(Expression<Func<RoutineExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<RoutineExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<RoutineExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<(IEnumerable<RoutineExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateAsync(UpdateRoutineExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
