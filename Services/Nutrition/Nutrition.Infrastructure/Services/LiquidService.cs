using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
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
            _liquidRepository = liquidRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<LiquidDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _liquidRepository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar líquidos");
                return Result.Failure<int>(Error.Problem("Liquid", "CountError").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure("General", "NullData").Description);

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<int>(Error.Failure("Liquid", "NameRequired").Description);

                if (dto.QuantityMl <= 0)
                    return Result.Failure<int>(Error.Failure("Liquid", "InvalidQuantity").Description);

                Liquid entity = LiquidMapper.ToEntity(dto);
                await _liquidRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar líquido {@LiquidData}", dto);
                return Result.Failure<int>(Error.Problem("Liquid", "CreateError").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateLiquidDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "NullData").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "EmptyList").Description);

                // Validação em lote
                var invalidItems = dtos.Where(dto => string.IsNullOrWhiteSpace(dto.Name) || dto.QuantityMl <= 0).ToList();
                if (invalidItems.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Liquid", "InvalidItems").Description);

                IEnumerable<Liquid> entities = dtos.Select(LiquidMapper.ToEntity);
                await _liquidRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos líquidos");
                return Result.Failure<IEnumerable<int>>(Error.Problem("Liquid", "CreateRangeError").Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Liquid? entity = await _liquidRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Liquid", "NotFound").Description);

                await _liquidRepository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir líquido {LiquidId}", id);
                return Result.Failure<bool>(Error.Problem("Liquid", "DeleteError").Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("General", "EmptyList").Description);

                List<Liquid> entities = new();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    Liquid? entity = await _liquidRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.NotFound("Liquid", "SomeNotFound").Description);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Liquid", "AllNotFound").Description);

                foreach (var entity in entities)
                    await _liquidRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos líquidos {LiquidIds}", ids);
                return Result.Failure<bool>(Error.Problem("Liquid", "DeleteRangeError").Description);
            }
        }

        public async Task<Result<IEnumerable<LiquidDTO>>> FindAsync(Expression<Func<LiquidDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<LiquidDTO>>(Error.Failure("General", "NullData").Description);

                // Para simplificar, não implementa filtro por predicate no momento
                IEnumerable<Liquid?> entities = await _liquidRepository.GetAll(cancellationToken);
                IEnumerable<LiquidDTO> dtos = entities.Select(LiquidMapper.ToDTO!);

                return Result.Success<IEnumerable<LiquidDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar líquidos com predicado");
                return Result.Failure<IEnumerable<LiquidDTO>>(Error.Problem("Liquid", "FindError").Description);
            }
        }

        public async Task<Result<IEnumerable<LiquidDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Liquid?> entities = await _liquidRepository.GetAll(cancellationToken);
                IEnumerable<LiquidDTO> dtos = entities.Select(LiquidMapper.ToDTO!);

                return Result.Success<IEnumerable<LiquidDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os líquidos");
                return Result.Failure<IEnumerable<LiquidDTO>>(Error.Problem("Liquid", "GetAllError").Description);
            }
        }

        public async Task<Result<LiquidDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<LiquidDTO>(Error.Failure("General", "InvalidId").Description);

                Liquid? entity = await _liquidRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<LiquidDTO>(Error.NotFound("Liquid", "NotFound").Description);

                LiquidDTO dto = LiquidMapper.ToDTO(entity);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar líquido {LiquidId}", id);
                return Result.Failure<LiquidDTO>(Error.Problem("Liquid", "GetByIdError").Description);
            }
        }

        public async Task<Result<(IEnumerable<LiquidDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<LiquidDTO>, int)>(Error.Failure("Liquid", "InvalidPage").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<LiquidDTO>, int)>(Error.Failure("Liquid", "InvalidPageSize").Description);

                IEnumerable<Liquid?> all = await _liquidRepository.GetAll(cancellationToken);
                int totalCount = all.Count();

                IEnumerable<LiquidDTO> items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(LiquidMapper.ToDTO!);

                return Result.Success<(IEnumerable<LiquidDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de líquidos (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<LiquidDTO>, int)>(Error.Problem("Liquid", "GetPagedError").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Liquid? entity = await _liquidRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Liquid", "NotFound").Description);

                // Validações adicionais
                if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<bool>(Error.Failure("Liquid", "NameRequired").Description);

                if (dto.QuantityMl != null && dto.QuantityMl <= 0)
                    return Result.Failure<bool>(Error.Failure("Liquid", "InvalidQuantity").Description);

                if (dto.CaloriesPerMl != null && dto.CaloriesPerMl < 0)
                    return Result.Failure<bool>(Error.Failure("Liquid", "NegativeCalories").Description);

                if (dto.Name != null)
                    entity.SetName(dto.Name);
                if (dto.QuantityMl != null)
                    entity.SetQuantityMl(dto.QuantityMl);
                if (dto.CaloriesPerMl != null)
                    entity.SetCaloriesPerMl(dto.CaloriesPerMl);

                entity.MarkAsUpdated();

                await _liquidRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar líquido {@LiquidData}", dto);
                return Result.Failure<bool>(Error.Problem("Liquid", "UpdateError").Description);
            }
        }
    }
}