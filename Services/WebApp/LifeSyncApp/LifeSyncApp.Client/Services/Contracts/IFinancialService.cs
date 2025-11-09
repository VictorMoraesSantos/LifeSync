using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Financial;
using LifeSyncApp.Client.Models.Financial.Category;
using LifeSyncApp.Client.Models.Financial.Transaction;

namespace LifeSyncApp.Client.Services.Contracts
{
    public interface IFinancialService
    {
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsAsync();
        Task<ApiResponse<int>> CreateTransactionAsync(CreateTransactionDTO command);
        Task<ApiResponse<bool>> UpdateTransactionAsync(UpdateTransactionDTO command);
        Task<ApiResponse<object>> DeleteTransactionAsync(int id);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesAsync();
        Task<ApiResponse<int>> CreateCategoryAsync(CreateCategoryDTO command);
        Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryDTO command);
        Task<ApiResponse<object>> DeleteCategoryAsync(int id);

        Task<ApiResponse<List<TransactionDTO>>> SearchTransactionsAsync(object filter);
        Task<ApiResponse<List<CategoryDTO>>> SearchCategoriesAsync(object filter);
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsByUserAsync(int userId);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesByUserAsync(int userId);
    }
}
