using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class LiquidService : ILiquidService
    {
        private readonly ILiquidRepository _liquidRepository;
        private readonly ILogger<LiquidService> _logger;

        public LiquidService(
            ILiquidRepository liquidRepository,
            ILogger<LiquidService> logger)
        {
            _liquidRepository = liquidRepository ?? throw new ArgumentNullException(nameof(liquidRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<LiquidDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var liquid = await _liquidRepository.GetById(id, cancellationToken);
                if (liquid == null)
                    return Result.Failure<LiquidDTO>(LiquidErrors.NotFound(id));

                var liquidDTO = LiquidMapper.ToDTO(liquid);

                return Result.Success(liquidDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar líquido {LiquidId}", id);
                return Result.Failure<LiquidDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<LiquidDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<LiquidDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _liquidRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(LiquidMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<LiquidDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de líquidos (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<LiquidDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<LiquidDTO>>> FindAsync(Expression<Func<LiquidDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<LiquidDTO>>(Error.NullValue);

                var entities = await _liquidRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(LiquidMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<LiquidDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar líquidos com predicado");
                return Result.Failure<IEnumerable<LiquidDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<LiquidDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _liquidRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(LiquidMapper.ToDTO)
                    .AsQueryable();
                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar líquidos");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<LiquidDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _liquidRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<LiquidDTO>>(new List<LiquidDTO>());

                var dtos = entities.Where(e => e != null).Select(LiquidMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<LiquidDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os líquidos");
                return Result.Failure<IEnumerable<LiquidDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = LiquidMapper.ToEntity(dto);
                await _liquidRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Líquido criado com sucesso: {LiquidId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar líquido {@LiquidData}", dto);
                return Result.Failure<int>(LiquidErrors.CreateError);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateLiquidDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de líquidos inválida ou vazia"));

                var entities = new List<Liquid>();
                var errors = new List<(string Name, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = LiquidMapper.ToEntity(dto);
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
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns líquidos possuem dados inválidos: {errorDetails}"));
                }

                await _liquidRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} líquidos com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos líquidos");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _liquidRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(LiquidErrors.NotFound(dto.Id));


                entity.Update(dto.Name, dto.QuantityMl, dto.CaloriesPerMl);

                await _liquidRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Líquido atualizado com sucesso: {LiquidId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar líquido {LiquidId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _liquidRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(LiquidErrors.NotFound(id));

                await _liquidRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Líquido excluído com sucesso: {LiquidId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir líquido {LiquidId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Liquid>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _liquidRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes líquidos não foram encontrados: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos líquidos foi encontrado"));

                foreach (var entity in entities)
                {
                    await _liquidRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} líquidos com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos líquidos {LiquidIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}