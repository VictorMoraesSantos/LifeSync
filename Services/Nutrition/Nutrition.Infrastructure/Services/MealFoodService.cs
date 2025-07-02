using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class MealFoodService : IMealFoodService
    {
        private readonly IMealFoodRepository _mealFoodRepository;
        private readonly ILogger<MealFoodService> _logger;

        public MealFoodService(
            IMealFoodRepository mealFoodRepository,
            ILogger<MealFoodService> logger)
        {
            _mealFoodRepository = mealFoodRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<MealFoodDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _mealFoodRepository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar alimentos de refeição");
                return Result.Failure<int>(Error.Problem("MealFood", "CountError").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure("General", "NullData").Description);

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(Error.Failure("MealFood", "NameRequired").Description);

                if (dto.QuantityInGrams <= 0)
                    return Result.Failure<int>(Error.Failure("MealFood", "InvalidQuantity").Description);

                if (dto.CaloriesPerUnit < 0)
                    return Result.Failure<int>(Error.Failure("MealFood", "NegativeCalories").Description);

                MealFood entity = MealFoodMapper.ToEntity(dto);
                await _mealFoodRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<int>(Error.Problem("MealFood", "CreateError").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateMealFoodDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "NullData").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "EmptyList").Description);

                // Validação em lote
                var invalidItems = dtos.Where(dto =>
                    string.IsNullOrWhiteSpace(dto.Name) ||
                    dto.QuantityInGrams <= 0 ||
                    dto.CaloriesPerUnit < 0).ToList();

                if (invalidItems.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("MealFood", "InvalidItems").Description);

                IEnumerable<MealFood> entities = dtos.Select(MealFoodMapper.ToEntity);
                await _mealFoodRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(entity => entity.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos alimentos de refeição");
                return Result.Failure<IEnumerable<int>>(Error.Problem("MealFood", "CreateRangeError").Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                MealFood? entity = await _mealFoodRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("MealFood", "NotFound").Description);

                await _mealFoodRepository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir alimento de refeição {MealFoodId}", id);
                return Result.Failure<bool>(Error.Problem("MealFood", "DeleteError").Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("General", "EmptyList").Description);

                List<MealFood> entities = new();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    MealFood? entity = await _mealFoodRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.NotFound("MealFood", "SomeNotFound").Description);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("MealFood", "AllNotFound").Description);

                foreach (var entity in entities)
                    await _mealFoodRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos alimentos de refeição {MealFoodIds}", ids);
                return Result.Failure<bool>(Error.Problem("MealFood", "DeleteRangeError").Description);
            }
        }

        public async Task<Result<IEnumerable<MealFoodDTO>>> FindAsync(Expression<Func<MealFoodDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<MealFoodDTO>>(Error.Failure("General", "NullData").Description);

                // Para simplificar, não implementa filtro por predicate no momento
                IEnumerable<MealFood?> entities = await _mealFoodRepository.GetAll(cancellationToken);
                IEnumerable<MealFoodDTO> dtos = entities.Select(MealFoodMapper.ToDTO!);

                return Result.Success<IEnumerable<MealFoodDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar alimentos de refeição com predicado");
                return Result.Failure<IEnumerable<MealFoodDTO>>(Error.Problem("MealFood", "FindError").Description);
            }
        }

        public async Task<Result<IEnumerable<MealFoodDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<MealFood?> entities = await _mealFoodRepository.GetAll(cancellationToken);
                IEnumerable<MealFoodDTO> dtos = entities.Select(MealFoodMapper.ToDTO!);

                return Result.Success<IEnumerable<MealFoodDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os alimentos de refeição");
                return Result.Failure<IEnumerable<MealFoodDTO>>(Error.Problem("MealFood", "GetAllError").Description);
            }
        }

        public async Task<Result<MealFoodDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<MealFoodDTO>(Error.Failure("General", "InvalidId").Description);

                MealFood? entity = await _mealFoodRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<MealFoodDTO>(Error.NotFound("MealFood", "NotFound").Description);

                MealFoodDTO dto = MealFoodMapper.ToDTO(entity);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar alimento de refeição {MealFoodId}", id);
                return Result.Failure<MealFoodDTO>(Error.Problem("MealFood", "GetByIdError").Description);
            }
        }

        public async Task<Result<(IEnumerable<MealFoodDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<MealFoodDTO>, int)>(Error.Failure("MealFood", "InvalidPage").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<MealFoodDTO>, int)>(Error.Failure("MealFood", "InvalidPageSize").Description);

                IEnumerable<MealFood?> all = await _mealFoodRepository.GetAll(cancellationToken);
                int totalCount = all.Count();

                IEnumerable<MealFoodDTO> items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MealFoodMapper.ToDTO!);

                return Result.Success<(IEnumerable<MealFoodDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de alimentos de refeição (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<MealFoodDTO>, int)>(Error.Problem("MealFood", "GetPagedError").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                MealFood? entity = await _mealFoodRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("MealFood", "NotFound").Description);

                // Validações
                if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<bool>(Error.Failure("MealFood", "NameRequired").Description);

                if (dto.QuantityInGrams != null && dto.QuantityInGrams <= 0)
                    return Result.Failure<bool>(Error.Failure("MealFood", "InvalidQuantity").Description);

                if (dto.CaloriesPerUnit != null && dto.CaloriesPerUnit < 0)
                    return Result.Failure<bool>(Error.Failure("MealFood", "NegativeCalories").Description);

                // Atualizações
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    entity.SetName(dto.Name);

                if (dto.QuantityInGrams > 0)
                    entity.SetQuantity(dto.QuantityInGrams);

                if (dto.CaloriesPerUnit > 0)
                    entity.SetCaloriesPerUnit(dto.CaloriesPerUnit);

                entity.MarkAsUpdated();

                await _mealFoodRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<bool>(Error.Problem("MealFood", "UpdateError").Description);
            }
        }
    }
}