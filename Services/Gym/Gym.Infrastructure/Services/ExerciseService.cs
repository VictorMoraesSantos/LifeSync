using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Exercise;
using Gym.Application.Mapping;
using Gym.Domain.Errors;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly ILogger<ExerciseService> _logger;

        public ExerciseService(IExerciseRepository exerciseRepository, ILogger<ExerciseService> logger)
        {
            _exerciseRepository = exerciseRepository;
            _logger = logger;
        }

        public async Task<Result<ExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _exerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<ExerciseDTO?>(ExerciseErrors.NotFound(id));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar exercício {Id}", id);
                return Result<ExerciseDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public Task<Result<int>> CountAsync(Expression<Func<ExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> CreateAsync(CreateExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateExerciseDTO> dto, CancellationToken cancellationToken = default)
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

        public Task<Result<IEnumerable<ExerciseDTO?>>> FindAsync(Expression<Func<ExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<ExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<(IEnumerable<ExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<ExerciseDTO?>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _exerciseRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => e.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<ExerciseDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex) { 
                _logger.LogError(ex, "Erro ao buscar exercícios com paginação");
                return Result<(IEnumerable<ExerciseDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public Task<Result<bool>> UpdateAsync(UpdateExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
