using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Filters;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;
        private readonly IPublisher _publisher;
        private readonly ILogger<MealService> _logger;

        public MealService(
            IMealRepository mealRepository,
            IPublisher publisher,
            ILogger<MealService> logger)
        {
            _mealRepository = mealRepository ?? throw new ArgumentNullException(nameof(mealRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<MealDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var meal = await _mealRepository.GetById(id, cancellationToken);
                if (meal == null)
                    return Result.Failure<MealDTO>(MealErrors.NotFound(id));

                var mealDTO = meal.ToDTO();

                return Result.Success(mealDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeição {MealId}", id);
                return Result.Failure<MealDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<MealDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<MealDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _mealRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(MealMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<MealDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de refeições (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<MealDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<MealDTO>>> FindAsync(Expression<Func<MealDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<MealDTO>>(Error.NullValue);

                var entities = await _mealRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(MealMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<MealDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeições com predicado");
                return Result.Failure<IEnumerable<MealDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<MealDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _mealRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(MealMapper.ToDTO)
                    .AsQueryable();
                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar refeições");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<MealDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _mealRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<MealDTO>>(new List<MealDTO>());

                var dtos = entities.Where(e => e != null).Select(MealMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<MealDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as refeições");
                return Result.Failure<IEnumerable<MealDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = MealMapper.ToEntity(dto);

                await _mealRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Refeição criada com sucesso: {MealId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar refeição {@MealData}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateMealDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de refeições inválida ou vazia"));

                var entities = new List<Meal>();
                var errors = new List<(string Name, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = MealMapper.ToEntity(dto);
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.Name ?? "Sem nome", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Name}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Algumas refeições possuem dados inválidos: {errorDetails}"));
                }

                await _mealRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criadas {Count} refeições com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas refeições");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _mealRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(MealErrors.NotFound(dto.Id));

                entity.Update(dto.Name, dto.Description);

                entity.MarkAsUpdated();

                await _mealRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Refeição atualizada com sucesso: {MealId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar refeição {MealId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _mealRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(MealErrors.NotFound(id));

                await _mealRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Refeição excluída com sucesso: {MealId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir refeição {MealId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Meal>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _mealRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"As seguintes refeições não foram encontradas: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhuma das refeições foi encontrada"));

                foreach (var entity in entities)
                {
                    await _mealRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídas {Count} refeições com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas refeições {MealIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> AddMealFoodAsync(int mealId, CreateMealFoodDTO mealFood, CancellationToken cancellationToken = default)
        {
            try
            {
                if (mealFood == null)
                    return Result.Failure<bool>(MealErrors.NullMealFood);

                var meal = await _mealRepository.GetById(mealId, cancellationToken);
                if (meal == null)
                    return Result.Failure<bool>(MealErrors.NotFound(mealId));

                var mealFoodEntity = MealFoodMapper.ToEntity(mealFood);

                meal.AddMealFood(mealFoodEntity);

                await _mealRepository.Update(meal, cancellationToken);

                foreach (var domainEvent in meal.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }

                meal.ClearDomainEvents();

                _logger.LogInformation("Alimento adicionado à refeição {MealId} com sucesso", mealId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar alimento à refeição {MealId}", mealId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> RemoveMealFoodAsync(int mealId, int foodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var meal = await _mealRepository.GetById(mealId, cancellationToken);
                if (meal == null)
                    return Result.Failure<bool>(MealErrors.NotFound(mealId));

                if (!meal.MealFoods.Any(mf => mf.Id == foodId))
                    return Result.Failure<bool>(MealErrors.MealFoodNotFound);

                meal.RemoveMealFood(foodId);

                await _mealRepository.Update(meal, cancellationToken);

                foreach (var domainEvent in meal.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }

                meal.ClearDomainEvents();

                _logger.LogInformation("Alimento {FoodId} removido da refeição {MealId} com sucesso", foodId, mealId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover alimento {FoodId} da refeição {MealId}", foodId, mealId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<MealDTO> Items, PaginationData Pagination)>> GetByFilterAsync(MealQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new MealQueryFilter(
                    filter.Id,
                    filter.NameContains,
                    filter.DescriptionContains,
                    filter.DiaryId,
                    filter.TotalCaloriesEquals,
                    filter.TotalCaloriesGreaterThan,
                    filter.TotalCaloriesLessThan,
                    filter.MealFoodId,
                    filter.CreatedAt,
                    filter.UpdatedAt,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _mealRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<MealDTO> Items, PaginationData Pagination)>(
                        (new List<MealDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(MealMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<MealDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar meals com filtro {@Filter}", filter);
                return Result.Failure<(IEnumerable<MealDTO>, PaginationData)>(Error.Failure(ex.Message));
            }
        }
    }
}