using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Repositories;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> CountAsync(Expression<Func<CategoryDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _categoryRepository.GetAll(cancellationToken);
            return all.Count();

        }

        public async Task<int> CreateAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = CategoryMapper.ToEntity(dto);

            await _categoryRepository.Create(entity, cancellationToken);

            return entity.Id;
        }

        public async Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<CreateCategoryDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entities = dto.Select(CategoryMapper.ToEntity).ToList();

            await _categoryRepository.CreateRange(entities, cancellationToken);

            return entities.Select(e => e.Id);
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            if (dto <= 0) throw new ArgumentOutOfRangeException(nameof(dto), "ID must be greater than zero.");

            var entity = await _categoryRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;

            await _categoryRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any()) return false;

            List<Category> entities = new();
            foreach (var id in ids)
            {
                Category? entity = await _categoryRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _categoryRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<IEnumerable<CategoryDTO>> FindAsync(Expression<Func<CategoryDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Category?> entities = await _categoryRepository.GetAll(cancellationToken);
            IEnumerable<CategoryDTO> dtos = entities.Select(CategoryMapper.ToDTO);
            return dtos;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Category?> entities = await _categoryRepository.GetAll(cancellationToken);
            IEnumerable<CategoryDTO> dtos = entities.Select(CategoryMapper.ToDTO);
            return dtos;
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");

            Category? entity = await _categoryRepository.GetById(id, cancellationToken);

            CategoryDTO? dto = CategoryMapper.ToDTO(entity);

            return dto;
        }

        public async Task<IEnumerable<CategoryDTO?>> GetByNameAsync(string name, int userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Name cannot be null or whitespace.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");

            IEnumerable<Category?> entities = await _categoryRepository.GetByNameContains(name, cancellationToken);
            IEnumerable<CategoryDTO?> dtos = entities.Select(CategoryMapper.ToDTO);

            return dtos;
        }

        public async Task<IEnumerable<CategoryDTO?>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");

            IEnumerable<Category?> entities = await _categoryRepository.GetAllByUserId(userId, cancellationToken);
            IEnumerable<CategoryDTO?> dtos = entities.Select(CategoryMapper.ToDTO);

            return dtos;
        }

        public async Task<(IEnumerable<CategoryDTO> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            IEnumerable<Category?> entities = await _categoryRepository.GetAll(cancellationToken);
            int totalCount = entities.Count();
            IEnumerable<CategoryDTO> dtos = entities.Select(CategoryMapper.ToDTO)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return (dtos, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) throw new ArgumentOutOfRangeException(nameof(dto.Id), "ID must be greater than zero.");

            var entity = await _categoryRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            entity.Update(dto.Name, dto.Description);

            await _categoryRepository.Update(entity, cancellationToken);

            return true;
        }
    }
}
