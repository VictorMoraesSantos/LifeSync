using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
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
            _mealFoodRepository = mealFoodRepository ?? throw new ArgumentNullException(nameof(mealFoodRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<MealFoodDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var mealFood = await _mealFoodRepository.GetById(id, cancellationToken);
                if (mealFood == null)
                    return Result.Failure<MealFoodDTO>(MealFoodErrors.NotFound(id));

                var mealFoodDTO = MealFoodMapper.ToDTO(mealFood);
                return Result.Success(mealFoodDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar alimento de refeição {MealFoodId}", id);
                return Result.Failure<MealFoodDTO>(MealFoodErrors.NotFound(id));
            }
        }

        public async Task<Result<(IEnumerable<MealFoodDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<MealFoodDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _mealFoodRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();

                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(MealFoodMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<MealFoodDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de alimentos de refeição (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<MealFoodDTO>, int)>(Error.Problem("Erro ao obter página de alimentos de refeição"));
            }
        }

        public async Task<Result<IEnumerable<MealFoodDTO>>> FindAsync(Expression<Func<MealFoodDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<MealFoodDTO>>(Error.NullValue);

                var entities = await _mealFoodRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(MealFoodMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<MealFoodDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar alimentos de refeição com predicado");
                return Result.Failure<IEnumerable<MealFoodDTO>>(Error.Problem("Erro ao buscar alimentos de refeição"));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<MealFoodDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _mealFoodRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(MealFoodMapper.ToDTO)
                    .AsQueryable();

                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar alimentos de refeição");
                return Result.Failure<int>(Error.Problem("Erro ao contar alimentos de refeição"));
            }
        }

        public async Task<Result<IEnumerable<MealFoodDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _mealFoodRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<MealFoodDTO>>(new List<MealFoodDTO>());

                var dtos = entities.Where(e => e != null).Select(MealFoodMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<MealFoodDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os alimentos de refeição");
                return Result.Failure<IEnumerable<MealFoodDTO>>(Error.Problem("Erro ao buscar todos os alimentos de refeição"));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = MealFoodMapper.ToEntity(dto);
                await _mealFoodRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Alimento de refeição criado com sucesso: {MealFoodId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (ArgumentException ex) when (ex.ParamName == "value")
            {
                _logger.LogWarning(ex, "Nome inválido ao criar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<int>(MealFoodErrors.InvalidName);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "quantity")
            {
                _logger.LogWarning(ex, "Quantidade inválida ao criar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<int>(MealFoodErrors.InvalidQuantity);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "caloriesPerUnit")
            {
                _logger.LogWarning(ex, "Calorias inválidas ao criar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<int>(MealFoodErrors.NegativeCalories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar alimento de refeição {@MealFoodData}", dto);
                return Result.Failure<int>(MealFoodErrors.CreateError);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateMealFoodDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de alimentos de refeição inválida ou vazia"));

                var entities = new List<MealFood>();
                var errors = new List<(string Name, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = MealFoodMapper.ToEntity(dto);
                        entities.Add(entity);
                    }
                    catch (ArgumentException ex)
                    {
                        errors.Add((dto.Name ?? "Sem nome", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Name}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns alimentos de refeição possuem dados inválidos: {errorDetails}"));
                }

                await _mealFoodRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} alimentos de refeição com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos alimentos de refeição");
                return Result.Failure<IEnumerable<int>>(Error.Problem("Erro ao criar múltiplos alimentos de refeição"));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _mealFoodRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(MealFoodErrors.NotFound(dto.Id));

                entity.MarkAsUpdated();
                await _mealFoodRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Alimento de refeição atualizado com sucesso: {MealFoodId}", dto.Id);
                return Result.Success(true);
            }
            catch (ArgumentException ex) when (ex.ParamName == "value")
            {
                _logger.LogWarning(ex, "Nome inválido ao atualizar alimento de refeição {MealFoodId}", dto.Id);
                return Result.Failure<bool>(MealFoodErrors.InvalidName);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "quantity")
            {
                _logger.LogWarning(ex, "Quantidade inválida ao atualizar alimento de refeição {MealFoodId}", dto.Id);
                return Result.Failure<bool>(MealFoodErrors.InvalidQuantity);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "caloriesPerUnit")
            {
                _logger.LogWarning(ex, "Calorias inválidas ao atualizar alimento de refeição {MealFoodId}", dto.Id);
                return Result.Failure<bool>(MealFoodErrors.NegativeCalories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar alimento de refeição {MealFoodId}", dto.Id);
                return Result.Failure<bool>(MealFoodErrors.UpdateError);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _mealFoodRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(MealFoodErrors.NotFound(id));

                await _mealFoodRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Alimento de refeição excluído com sucesso: {MealFoodId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir alimento de refeição {MealFoodId}", id);
                return Result.Failure<bool>(MealFoodErrors.DeleteError);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<MealFood>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _mealFoodRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes alimentos de refeição não foram encontrados: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos alimentos de refeição foi encontrado"));

                foreach (var entity in entities)
                {
                    await _mealFoodRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} alimentos de refeição com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos alimentos de refeição {MealFoodIds}", ids);
                return Result.Failure<bool>(Error.Problem(MealFoodErrors.DeleteError.Description));
            }
        }
    }
}