using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
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
            _mealRepository = mealRepository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<Result<bool>> AddMealFoodAsync(int mealId, CreateMealFoodDTO mealFood, CancellationToken cancellationToken)
        {
            try
            {
                if (mealId <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (mealFood == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (string.IsNullOrWhiteSpace(mealFood.Name))
                    return Result.Failure<bool>(Error.Failure("MealFood", "NameRequired").Description);

                if (mealFood.QuantityInGrams <= 0)
                    return Result.Failure<bool>(Error.Failure("MealFood", "InvalidQuantity").Description);

                Meal? meal = await _mealRepository.GetById(mealId, cancellationToken);
                if (meal == null)
                    return Result.Failure<bool>(Error.NotFound("Meal", "NotFound").Description);

                MealFood? mealFoodEntity = MealFoodMapper.ToEntity(mealFood);

                meal.AddMealFood(mealFoodEntity);
                await _mealRepository.Update(meal, cancellationToken);

                // Publicar eventos de domínio
                foreach (var domainEvent in meal.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }
                meal.ClearDomainEvents();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar alimento à refeição {MealId}", mealId);
                return Result.Failure<bool>(Error.Problem("Meal", "AddFoodError").Description);
            }
        }

        public async Task<Result<bool>> RemoveMealFoodAsync(int mealId, int foodId, CancellationToken cancellationToken)
        {
            try
            {
                if (mealId <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (foodId <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Meal? meal = await _mealRepository.GetById(mealId, cancellationToken);
                if (meal == null)
                    return Result.Failure<bool>(Error.NotFound("Meal", "NotFound").Description);

                if (!meal.MealFoods.Any(mf => mf.Id == foodId))
                    return Result.Failure<bool>(Error.NotFound("MealFood", "NotFound").Description);

                meal.RemoveMealFood(foodId);
                await _mealRepository.Update(meal, cancellationToken);

                // Publicar eventos de domínio
                foreach (var domainEvent in meal.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }
                meal.ClearDomainEvents();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover alimento {FoodId} da refeição {MealId}", foodId, mealId);
                return Result.Failure<bool>(Error.Problem("Meal", "RemoveFoodError").Description);
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<MealDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _mealRepository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar refeições");
                return Result.Failure<int>(Error.Problem("Meal", "CountError").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure("General", "NullData").Description);

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(Error.Failure("Meal", "NameRequired").Description);

                Meal entity = MealMapper.ToEntity(dto);
                await _mealRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar refeição {@MealData}", dto);
                return Result.Failure<int>(Error.Problem("Meal", "CreateError").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateMealDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "NullData").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "EmptyList").Description);

                // Validação em lote
                var invalidItems = dtos.Where(dto => string.IsNullOrWhiteSpace(dto.Name)).ToList();
                if (invalidItems.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Meal", "InvalidItems").Description);

                IEnumerable<Meal> entities = dtos.Select(MealMapper.ToEntity);
                await _mealRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas refeições");
                return Result.Failure<IEnumerable<int>>(Error.Problem("Meal", "CreateRangeError").Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Meal? entity = await _mealRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Meal", "NotFound").Description);

                await _mealRepository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir refeição {MealId}", id);
                return Result.Failure<bool>(Error.Problem("Meal", "DeleteError").Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("General", "EmptyList").Description);

                List<Meal> entities = new();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    Meal? entity = await _mealRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.NotFound("Meal", "SomeNotFound").Description);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Meal", "AllNotFound").Description);

                foreach (var entity in entities)
                    await _mealRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas refeições {MealIds}", ids);
                return Result.Failure<bool>(Error.Problem("Meal", "DeleteRangeError").Description);
            }
        }

        public async Task<Result<IEnumerable<MealDTO>>> FindAsync(Expression<Func<MealDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<MealDTO>>(Error.Failure("General", "NullData").Description);

                // Para simplificar, não implementa filtro por predicate no momento
                IEnumerable<Meal?> entities = await _mealRepository.GetAll(cancellationToken);
                IEnumerable<MealDTO> dtos = entities.Select(MealMapper.ToDTO!);

                return Result.Success<IEnumerable<MealDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeições com predicado");
                return Result.Failure<IEnumerable<MealDTO>>(Error.Problem("Meal", "FindError").Description);
            }
        }

        public async Task<Result<IEnumerable<MealDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Meal?> entities = await _mealRepository.GetAll(cancellationToken);
                IEnumerable<MealDTO> dtos = entities.Select(MealMapper.ToDTO!);

                return Result.Success<IEnumerable<MealDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as refeições");
                return Result.Failure<IEnumerable<MealDTO>>(Error.Problem("Meal", "GetAllError").Description);
            }
        }

        public async Task<Result<MealDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<MealDTO>(Error.Failure("General", "InvalidId").Description);

                Meal? entity = await _mealRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<MealDTO>(Error.NotFound("Meal", "NotFound").Description);

                MealDTO dto = MealMapper.ToDTO(entity);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeição {MealId}", id);
                return Result.Failure<MealDTO>(Error.Problem("Meal", "GetByIdError").Description);
            }
        }

        public async Task<Result<(IEnumerable<MealDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<MealDTO>, int)>(Error.Failure("Meal", "InvalidPage").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<MealDTO>, int)>(Error.Failure("Meal", "InvalidPageSize").Description);

                IEnumerable<Meal?> all = await _mealRepository.GetAll(cancellationToken);
                int totalCount = all.Count();

                IEnumerable<MealDTO> items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MealMapper.ToDTO!);

                return Result.Success<(IEnumerable<MealDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de refeições (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<MealDTO>, int)>(Error.Problem("Meal", "GetPagedError").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Meal? entity = await _mealRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Meal", "NotFound").Description);

                // Validações
                if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<bool>(Error.Failure("Meal", "NameRequired").Description);

                // Atualizações
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    entity.UpdateName(dto.Name);

                if (!string.IsNullOrWhiteSpace(dto.Description))
                    entity.UpdateDescription(dto.Description);

                entity.MarkAsUpdated();

                await _mealRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar refeição {@MealData}", dto);
                return Result.Failure<bool>(Error.Problem("Meal", "UpdateError").Description);
            }
        }
    }
}