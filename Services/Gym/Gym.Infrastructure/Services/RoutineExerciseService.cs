using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Gym.Application.Contracts;
using Gym.Application.DTOs.RoutineExercise;
using Gym.Application.Mapping;
using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Services
{
    public class RoutineExerciseService : IRoutineExerciseService
    {
        private readonly IRoutineExerciseRepository _routineExerciseRepository;
        private readonly ILogger<RoutineExerciseService> _logger;

        public RoutineExerciseService(
            IRoutineExerciseRepository routineExerciseRepository,
            ILogger<RoutineExerciseService> logger)
        {
            _routineExerciseRepository = routineExerciseRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<RoutineExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<int>(Error.NullValue);

                var all = await _routineExerciseRepository.GetAll(cancellationToken);
                var count = all
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Count(predicate);

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting routine exercises with filter");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateRoutineExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _routineExerciseRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Routine exercise {Id} created successfully", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating routine exercise");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateRoutineExerciseDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<RoutineExercise>();
                var errors = new List<(string Title, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = dto.ToEntity();
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.ExerciseId.ToString(), ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Some routine exercises have invalid data: {errorDetails}"));
                }

                await _routineExerciseRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Created {Count} routine exercises successfully", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple routine exercises");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _routineExerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Routine exercise with ID {id} not found"));

                await _routineExerciseRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Routine exercise {Id} deleted successfully", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting routine exercise {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Invalid or empty ID list"));

                var entities = new List<RoutineExercise>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _routineExerciseRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"The following routine exercises were not found: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("None of the routine exercises were found"));

                foreach (var entity in entities)
                {
                    await _routineExerciseRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Deleted {Count} routine exercises successfully", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple routine exercises {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoutineExerciseDTO?>>> FindAsync(Expression<Func<RoutineExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<RoutineExerciseDTO?>>(Error.NullValue);

                var entities = await _routineExerciseRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<RoutineExerciseDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding routine exercises with filter");
                return Result<IEnumerable<RoutineExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoutineExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _routineExerciseRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<RoutineExerciseDTO?>>(new List<RoutineExerciseDTO>());

                var dtos = entities.Select(e => e.ToDTO()!).ToList();

                return Result.Success<IEnumerable<RoutineExerciseDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all routine exercises");
                return Result<IEnumerable<RoutineExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RoutineExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _routineExerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<RoutineExerciseDTO?>(Error.NotFound($"Routine exercise with ID {id} not found"));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting routine exercise {Id}", id);
                return Result<RoutineExerciseDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<RoutineExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<RoutineExerciseDTO?>, int)>(Error.Failure("Invalid pagination parameters"));

                var entities = await _routineExerciseRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => e.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<RoutineExerciseDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged routine exercises");
                return Result<(IEnumerable<RoutineExerciseDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateRoutineExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _routineExerciseRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Routine exercise with ID {dto.Id} not found"));

                entity.Update(
                    dto.Sets,
                    dto.Repetitions,
                    dto.RestBetweenSets,
                    dto.RecommendedWeight,
                    dto.Instructions);

                await _routineExerciseRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Routine exercise {Id} updated successfully", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating routine exercise {Id}", dto?.Id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
