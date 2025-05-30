using Financial.Application.DTOs.Category;
using Financial.Domain.Entities;

namespace Financial.Application.Mappings
{
    public static class CategoryMapper
    {
        public static Category ToEntity(CreateCategoryDTO dto)
        {
            Category category = new(dto.UserId, dto.Name, dto.Description);
            return category;
        }

        public static CategoryDTO ToDTO(Category entity)
        {
            CategoryDTO dto = new(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Description);
            return dto;
        }
    }
}
