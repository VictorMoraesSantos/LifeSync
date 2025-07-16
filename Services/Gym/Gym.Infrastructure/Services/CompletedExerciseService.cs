using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Gym.Application.Contracts;
using Gym.Application.DTOs.CompletedExercise;
using Gym.Application.Mapping;
using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

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

        public async Task<Result<int>> CountAsync(Expression<Func<CompletedExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<int>(Error.NullValue);

                var all = await _completedExerciseRepository.GetAll(cancellationToken);
                var count = all
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Count(predicate);

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting completed exercises with filter");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateCompletedExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _completedExerciseRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Completed exercise {Id} created successfully", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating completed exercise");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateCompletedExerciseDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<CompletedExercise>();
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
                        errors.Add((dto.TrainingSessionId.ToString(), ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Some completed exercises have invalid data: {errorDetails}"));
                }

                await _completedExerciseRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Created {Count} completed exercises successfully", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple completed exercises");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _completedExerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Completed exercise with ID {id} not found"));

                await _completedExerciseRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Completed exercise {Id} deleted successfully", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting completed exercise {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Invalid or empty ID list"));

                var entities = new List<CompletedExercise>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _completedExerciseRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"The following completed exercises were not found: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("None of the completed exercises were found"));

                foreach (var entity in entities)
                {
                    await _completedExerciseRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Deleted {Count} completed exercises successfully", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple completed exercises {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CompletedExerciseDTO?>>> FindAsync(Expression<Func<CompletedExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<CompletedExerciseDTO?>>(Error.NullValue);

                var entities = await _completedExerciseRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<CompletedExerciseDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding completed exercises with filter");
                return Result<IEnumerable<CompletedExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CompletedExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _completedExerciseRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<CompletedExerciseDTO?>>(new List<CompletedExerciseDTO>());

                var dtos = entities.Select(e => e.ToDTO()!).ToList();

                return Result.Success<IEnumerable<CompletedExerciseDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all completed exercises");
                return Result<IEnumerable<CompletedExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<CompletedExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _completedExerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<CompletedExerciseDTO?>(Error.NotFound($"Completed exercise with ID {id} not found"));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting completed exercise {Id}", id);
                return Result<CompletedExerciseDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<CompletedExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<CompletedExerciseDTO?>, int)>(Error.Failure("Invalid pagination parameters"));

                var entities = await _completedExerciseRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => e.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<CompletedExerciseDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged completed exercises");
                return Result<(IEnumerable<CompletedExerciseDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateCompletedExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _completedExerciseRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Completed exercise with ID {dto.Id} not found"));

                entity.Update(
                    dto.SetsCompleted,
                    dto.RepetitionsCompleted,
                    dto.WeightUsed,
                    dto.Notes);

                await _completedExerciseRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Completed exercise {Id} updated successfully", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating completed exercise {Id}", dto?.Id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
