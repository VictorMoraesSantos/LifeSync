using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Financial;

namespace LifeSyncApp.Client.Services.Contracts
{
    public interface IFinancialService
    {
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsAsync();
        Task<ApiResponse<int>> CreateTransactionAsync(CreateTransactionCommand command);
        Task<ApiResponse<bool>> UpdateTransactionAsync(UpdateTransactionCommand command);
        Task<ApiResponse<object>> DeleteTransactionAsync(int id);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesAsync();
        Task<ApiResponse<int>> CreateCategoryAsync(CreateCategoryCommand command);
        Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryCommand command);
        Task<ApiResponse<object>> DeleteCategoryAsync(int id);

        Task<ApiResponse<List<TransactionDTO>>> SearchTransactionsAsync(object filter);
        Task<ApiResponse<List<CategoryDTO>>> SearchCategoriesAsync(object filter);
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsByUserAsync(int userId);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesByUserAsync(int userId);
    }
}
