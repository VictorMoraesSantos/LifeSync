using LifeSyncApp.Models.Financial.Category;

namespace LifeSyncApp.Services.Financial
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategoriesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<CategoryDTO?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<(int? Id, string? Error)> CreateCategoryAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> UpdateCategoryAsync(int id, UpdateCategoryDTO dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    }
}
