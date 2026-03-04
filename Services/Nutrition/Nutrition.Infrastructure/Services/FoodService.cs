using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.Contracts;
using Nutrition.Application.DTOs.Food;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Filters;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ILogger<Food> _logger;

        public FoodService(IFoodRepository foodRepository, ILogger<Food> logger)
        {
            _foodRepository = foodRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<FoodDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _foodRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(FoodMapper.ToDTO)
                    .AsQueryable();
                var count = predicate != null ? dtos.Count(predicate) : dtos.Count();

                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting Food entities");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _foodRepository.GetById(id, cancellationToken);
                if (entity == null) return Result.Failure<bool>(FoodErrors.NotFound(id));

                await _foodRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Deleted Food entity with ID {Id}", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Food entity with ID {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any()) return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Food>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _foodRepository.GetById(id, cancellationToken);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }
                    else
                    {
                        notFoundIds.Add(id);
                    }
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result<bool>.Failure(Error.Failure($"Não foram encontrados Food entities para os seguintes IDs: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.Failure("Nenhuma Food entity encontrada para os IDs fornecidos"));

                foreach (var entity in entities)
                {
                    await _foodRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} Foods com sucesso", entities.Count);
                return Result.Success<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir multiplos Foods com IDs: {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<FoodDTO?>>> FindAsync(Expression<Func<FoodDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null) return Result.Failure<IEnumerable<FoodDTO?>>(Error.NullValue);

                var entities = await _foodRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(FoodMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<FoodDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ao buscar food por predicate");
                return Result<IEnumerable<FoodDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<FoodDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _foodRepository.GetAll(cancellationToken);
                if (entities == null) return Result.Failure<IEnumerable<FoodDTO?>>(Error.NullValue);

                var dtos = entities.Where(e => e != null).Select(FoodMapper.ToDTO).ToList();

                return Result<IEnumerable<FoodDTO?>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ao buscar todas as foods");
                return Result<IEnumerable<FoodDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<FoodDTO> Items, PaginationData Pagination)>> GetByFilterAsync(FoodQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domailFilter = new FoodQueryFilter(
                    filter.Id,
                    filter.Name,
                    filter.CaloriesEquals,
                    filter.CaloriesGreaterThan,
                    filter.CaloriesLessThan,
                    filter.ProteinEquals,
                    filter.ProteinGreaterThan,
                    filter.ProteinLessThan,
                    filter.LipidsEquals,
                    filter.LipidsGreaterThan,
                    filter.LipidsLessThan,
                    filter.CarbohydratesEquals,
                    filter.CarbohydratesGreaterThan,
                    filter.CarbohydratesLessThan,
                    filter.CalciumEquals,
                    filter.CalciumGreaterThan,
                    filter.CalciumLessThan,
                    filter.MagnesiumEquals,
                    filter.MagnesiumGreaterThan,
                    filter.MagnesiumLessThan,
                    filter.IronEquals,
                    filter.IronGreaterThan,
                    filter.IronLessThan,
                    filter.SodiumEquals,
                    filter.SodiumGreaterThan,
                    filter.SodiumLessThan,
                    filter.PotassiumEquals,
                    filter.PotassiumGreaterThan,
                    filter.PotassiumLessThan,
                    filter.CreatedAt,
                    filter.UpdatedAt);

                var (entities, totalItems) = await _foodRepository.FindByFilter(domailFilter, cancellationToken);
                if (!entities.Any()) return Result.Success<(IEnumerable<FoodDTO> Items, PaginationData Pagination)>((new List<FoodDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Select(FoodMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<FoodDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ao buscar foods por filtro");
                return Result<(IEnumerable<FoodDTO>, PaginationData)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<FoodDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _foodRepository.GetById(id, cancellationToken);
                if (entity == null) return Result<FoodDTO?>.Failure(FoodErrors.NotFound(id));

                var dto = entity.ToDTO();

                return Result<FoodDTO?>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ao buscar food por ID {Id}", id);
                return Result<FoodDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<FoodDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1) return Result.Failure<(IEnumerable<FoodDTO?>, int)>(Error.Failure("Parametros de paginacao invalidos"));

                var entities = await _foodRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Where(e => e != null)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(FoodMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<FoodDTO?>, int)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de foods (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<FoodDTO?>, int)>(Error.Failure(ex.Message));
            }
        }
    }
}
