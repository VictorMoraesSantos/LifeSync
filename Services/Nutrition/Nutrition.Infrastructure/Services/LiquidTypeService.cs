using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.LiquidType;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Filters;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class LiquidTypeService : ILiquidTypeService
    {
        private readonly ILiquidTypeRepository _liquidTypeRepository;
        private readonly ILogger<LiquidTypeService> _logger;

        public LiquidTypeService(
            ILiquidTypeRepository liquidTypeRepository,
            ILogger<LiquidTypeService> logger)
        {
            _liquidTypeRepository = liquidTypeRepository;
            _logger = logger;
        }

        public async Task<Result<LiquidTypeDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var liquidType = await _liquidTypeRepository.GetById(id, cancellationToken);
                if (liquidType == null)
                    return Result.Failure<LiquidTypeDTO>(LiquidTypeErrors.NotFound(id));

                var liquidTypeDTO = LiquidTypeMapper.ToDTO(liquidType);

                return Result.Success(liquidTypeDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipo de líquido {LiquidTypeId}", id);
                return Result.Failure<LiquidTypeDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<LiquidTypeDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<LiquidTypeDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _liquidTypeRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(LiquidTypeMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<LiquidTypeDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de tipos de líquido (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<LiquidTypeDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<LiquidTypeDTO>>> FindAsync(Expression<Func<LiquidTypeDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<LiquidTypeDTO>>(Error.NullValue);

                var entities = await _liquidTypeRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(LiquidTypeMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<LiquidTypeDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipos de líquido com predicado");
                return Result.Failure<IEnumerable<LiquidTypeDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<LiquidTypeDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _liquidTypeRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(LiquidTypeMapper.ToDTO)
                    .AsQueryable();
                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar tipos de líquido");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<LiquidTypeDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _liquidTypeRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<LiquidTypeDTO>>(new List<LiquidTypeDTO>());

                var dtos = entities.Where(e => e != null).Select(LiquidTypeMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<LiquidTypeDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os tipos de líquido");
                return Result.Failure<IEnumerable<LiquidTypeDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateLiquidTypeDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = LiquidTypeMapper.ToEntity(dto);
                await _liquidTypeRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Tipo de líquido criado com sucesso: {LiquidTypeId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tipo de líquido {@LiquidTypeData}", dto);
                return Result.Failure<int>(LiquidTypeErrors.CreateError);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateLiquidTypeDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de tipos de líquido inválida ou vazia"));

                var entities = new List<LiquidType>();
                var errors = new List<string>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = LiquidTypeMapper.ToEntity(dto);
                        entities.Add(entity);
                    }
                    catch (ArgumentException ex)
                    {
                        errors.Add(ex.Message);
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors);
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns tipos de líquido possuem dados inválidos: {errorDetails}"));
                }

                await _liquidTypeRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} tipos de líquido com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos tipos de líquido");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateLiquidTypeDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _liquidTypeRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(LiquidTypeErrors.NotFound(dto.Id));

                entity.Update(dto.Name);

                await _liquidTypeRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Tipo de líquido atualizado com sucesso: {LiquidTypeId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tipo de líquido {LiquidTypeId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _liquidTypeRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(LiquidTypeErrors.NotFound(id));

                await _liquidTypeRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Tipo de líquido excluído com sucesso: {LiquidTypeId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tipo de líquido {LiquidTypeId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<LiquidType>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _liquidTypeRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes tipos de líquido não foram encontrados: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos tipos de líquido foi encontrado"));

                foreach (var entity in entities)
                {
                    await _liquidTypeRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} tipos de líquido com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos tipos de líquido {LiquidTypeIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<LiquidTypeDTO> Items, PaginationData Pagination)>> GetByFilterAsync(LiquidTypeQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new LiquidTypeQueryFilter(
                    filter.Id,
                    filter.NameContains,
                    filter.CreatedAt,
                    filter.UpdatedAt,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _liquidTypeRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<LiquidTypeDTO> Items, PaginationData Pagination)>((new List<LiquidTypeDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(LiquidTypeMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<LiquidTypeDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipos de líquido com filtro {@Filter}", filter);
                return Result.Failure<(IEnumerable<LiquidTypeDTO>, PaginationData)>(Error.Failure(ex.Message));
            }
        }
    }
}
