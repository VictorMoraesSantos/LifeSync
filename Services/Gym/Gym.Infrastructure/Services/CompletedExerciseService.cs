using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.CompletedExercise;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Services
{
    public class CompletedExerciseService : ICompletedExerciseService
    {
        private readonly ICompletedExerciseRepository _completedExerciseRepository;
        private readonly ILogger<ICompletedExerciseService> _logger;

        public CompletedExerciseService(ICompletedExerciseRepository completedExerciseRepository, ILogger<ICompletedExerciseService> logger)
        {
            _completedExerciseRepository = completedExerciseRepository;
            _logger = logger;
        }

        public Task<Result<int>> CountAsync(Expression<Func<CompletedExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> CreateAsync(CreateCompletedExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateCompletedExerciseDTO> dto, CancellationToken cancellationToken = default)
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

        public Task<Result<IEnumerable<CompletedExerciseDTO?>>> FindAsync(Expression<Func<CompletedExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<CompletedExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CompletedExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<(IEnumerable<CompletedExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateAsync(UpdateCompletedExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
