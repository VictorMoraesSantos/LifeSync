using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Errors;
using Financial.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<CategoryDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _categoryRepository.GetAll(cancellationToken);

                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar categorias");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _categoryRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria {@CategoryData}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateCategoryDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.NullValue);

                // Validação em lote
                var invalidItems = dtos.Where(dto => string.IsNullOrWhiteSpace(dto.Name)).ToList();
                if (invalidItems.Any())
                    return Result.Failure<IEnumerable<int>>(CategoryErrors.CreateError);

                var entities = dtos.Select(CategoryMapper.ToEntity).ToList();

                await _categoryRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas categorias");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("ID deve ser maior que zero"));

                var entity = await _categoryRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(CategoryErrors.NotFound(id));

                await _categoryRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria {CategoryId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Category>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _categoryRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.Failure($"As seguintes categorias não foram encontradas: {string.Join(", ", notFoundIds)}"));

                if (!entities.Any())
                    return Result.Failure<bool>(Error.Failure("Nenhuma das categorias foi encontrada"));

                foreach (var entity in entities)
                    await _categoryRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas categorias {CategoryIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> FindAsync(Expression<Func<CategoryDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<CategoryDTO>>(Error.NullValue);

                var entities = await _categoryRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(CategoryMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<CategoryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias com predicado");
                return Result.Failure<IEnumerable<CategoryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _categoryRepository.GetAll(cancellationToken);
                var dtos = entities.Select(CategoryMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<CategoryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                return Result.Failure<IEnumerable<CategoryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<CategoryDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<CategoryDTO>(CategoryErrors.InvalidId);

                var entity = await _categoryRepository.GetById(id, cancellationToken);
                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria {CategoryId}", id);
                return Result.Failure<CategoryDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetByNameAsync(string name, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<CategoryDTO>>(CategoryErrors.InvalidUserId);

                var entities = await _categoryRepository.GetByNameContains(name, cancellationToken);
                var dtos = entities.Select(CategoryMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<CategoryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias por nome {Name} e usuário {UserId}", name, userId);
                return Result.Failure<IEnumerable<CategoryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<CategoryDTO>>(CategoryErrors.InvalidUserId);

                var entities = await _categoryRepository.GetAllByUserId(userId, cancellationToken);
                var dtos = entities.Select(CategoryMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<CategoryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<CategoryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<CategoryDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page <= 0)
                    return Result.Failure<(IEnumerable<CategoryDTO>, int)>(Error.Failure("Página deve ser maior que zero"));

                if (pageSize <= 0)
                    return Result.Failure<(IEnumerable<CategoryDTO>, int)>(Error.Failure("Tamanho da página deve ser maior que zero"));

                var entities = await _categoryRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var dtos = entities.Select(CategoryMapper.ToDTO)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Result.Success<(IEnumerable<CategoryDTO> Items, int TotalCount)>((dtos, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de categorias (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<CategoryDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(CategoryErrors.InvalidId);

                var entity = await _categoryRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(CategoryErrors.NotFound(dto.Id));

                entity.Update(dto.Name, dto.Description);

                await _categoryRepository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria {@CategoryData}", dto);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}