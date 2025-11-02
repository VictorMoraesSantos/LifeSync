using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;
using Gym.Application.Mapping;
using Gym.Domain.Entities;
using Gym.Domain.Filters;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Services
{
    public class TrainingSessionService : ITrainingSessionService
    {
        private readonly ITrainingSessionRepository _trainingSessionRepository;
        private readonly ILogger<TrainingSessionService> _logger;

        public TrainingSessionService(
            ITrainingSessionRepository trainingSessionRepository,
            ILogger<TrainingSessionService> logger)
        {
            _trainingSessionRepository = trainingSessionRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<TrainingSessionDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<int>(Error.NullValue);

                var all = await _trainingSessionRepository.GetAll(cancellationToken);
                var count = all
                    .Where(ts => ts != null)
                    .Select(ts => ts.ToDTO())
                    .AsQueryable()
                    .Count(predicate);

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting training sessions with filter");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateTrainingSessionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _trainingSessionRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Training session {Id} created successfully", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training session");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTrainingSessionDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<TrainingSession>();
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
                        errors.Add((dto.RoutineId.ToString(), ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Some training sessions have invalid data: {errorDetails}"));
                }

                await _trainingSessionRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Created {Count} training sessions successfully", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple training sessions");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _trainingSessionRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Training session with ID {id} not found"));

                await _trainingSessionRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Training session {Id} deleted successfully", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training session {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Invalid or empty ID list"));

                var entities = new List<TrainingSession>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _trainingSessionRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"The following training sessions were not found: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("None of the training sessions were found"));

                foreach (var entity in entities)
                {
                    await _trainingSessionRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Deleted {Count} training sessions successfully", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple training sessions {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TrainingSessionDTO?>>> FindAsync(Expression<Func<TrainingSessionDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<TrainingSessionDTO?>>(Error.NullValue);

                var entities = await _trainingSessionRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(ts => ts != null)
                    .Select(ts => ts.ToDTO())
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<TrainingSessionDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding training sessions with filter");
                return Result<IEnumerable<TrainingSessionDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TrainingSessionDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _trainingSessionRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<TrainingSessionDTO?>>(new List<TrainingSessionDTO>());

                var dtos = entities.Select(ts => ts.ToDTO()!).ToList();

                return Result.Success<IEnumerable<TrainingSessionDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all training sessions");
                return Result<IEnumerable<TrainingSessionDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TrainingSessionDTO> Items, PaginationData Pagination)>> GetByFilterAsync(TrainingSessionFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new TrainingSessionQueryFilter(
                    filter.Id,
                    filter.UserId,
                    filter.RoutineId,
                    filter.StartTime,
                    filter.EndTime,
                    filter.NotesContains,
                    filter.CompletedExerciseId,
                    filter.CreatedAt,
                    filter.UpdatedAt,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _trainingSessionRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<TrainingSessionDTO>, PaginationData)>((new List<TrainingSessionDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(TrainingSessionMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<TrainingSessionDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training sessions by filter");
                return Result<(IEnumerable<TrainingSessionDTO>, PaginationData)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<TrainingSessionDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _trainingSessionRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<TrainingSessionDTO?>(Error.NotFound($"Training session with ID {id} not found"));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training session {Id}", id);
                return Result<TrainingSessionDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TrainingSessionDTO?>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _trainingSessionRepository.Find(ts => ts.UserId == userId, cancellationToken);
                var dtos = entities.Select(ts => ts.ToDTO()!).ToList();
                return Result.Success<IEnumerable<TrainingSessionDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training sessions for user {UserId}", userId);
                return Result<IEnumerable<TrainingSessionDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TrainingSessionDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<TrainingSessionDTO?>, int)>(Error.Failure("Invalid pagination parameters"));

                var entities = await _trainingSessionRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ts => ts.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<TrainingSessionDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged training sessions");
                return Result<(IEnumerable<TrainingSessionDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateTrainingSessionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _trainingSessionRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Training session with ID {dto.Id} not found"));

                entity.Complete(dto.Notes);

                await _trainingSessionRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Training session {Id} updated successfully", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training session {Id}", dto?.Id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
