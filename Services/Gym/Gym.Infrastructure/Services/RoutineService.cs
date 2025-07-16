using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Routine;
using Gym.Application.Mapping;
using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Services
{
    public class RoutineService : IRoutineService
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly ILogger<RoutineService> _logger;

        public RoutineService(
            IRoutineRepository routineRepository,
            ILogger<RoutineService> logger)
        {
            _routineRepository = routineRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<RoutineDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<int>(Error.NullValue);

                var all = await _routineRepository.GetAll(cancellationToken);
                var count = all
                    .Where(r => r != null)
                    .Select(r => r.ToDTO())
                    .AsQueryable()
                    .Count(predicate);

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting routines with filter");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateRoutineDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _routineRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Routine {Id} created successfully", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating routine");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateRoutineDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<Routine>();
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
                        errors.Add((dto.Name ?? "No name", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Some routines have invalid data: {errorDetails}"));
                }

                await _routineRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Created {Count} routines successfully", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple routines");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _routineRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Routine with ID {id} not found"));

                await _routineRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Routine {Id} deleted successfully", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting routine {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Invalid or empty ID list"));

                var entities = new List<Routine>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _routineRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"The following routines were not found: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("None of the routines were found"));

                foreach (var entity in entities)
                {
                    await _routineRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Deleted {Count} routines successfully", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple routines {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoutineDTO?>>> FindAsync(Expression<Func<RoutineDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<RoutineDTO?>>(Error.NullValue);

                var entities = await _routineRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(r => r != null)
                    .Select(r => r.ToDTO())
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<RoutineDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding routines with filter");
                return Result<IEnumerable<RoutineDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoutineDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _routineRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<RoutineDTO?>>(new List<RoutineDTO>());

                var dtos = entities.Select(r => r.ToDTO()!).ToList();

                return Result.Success<IEnumerable<RoutineDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all routines");
                return Result<IEnumerable<RoutineDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RoutineDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _routineRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<RoutineDTO?>(Error.NotFound($"Routine with ID {id} not found"));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting routine {Id}", id);
                return Result<RoutineDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<RoutineDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<RoutineDTO?>, int)>(Error.Failure("Invalid pagination parameters"));

                var entities = await _routineRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => r.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<RoutineDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged routines");
                return Result<(IEnumerable<RoutineDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateRoutineDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _routineRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound($"Routine with ID {dto.Id} not found"));

                entity.Update(dto.Name, dto.Description);

                await _routineRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Routine {Id} updated successfully", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating routine {Id}", dto?.Id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
