using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Financial;

namespace LifeSyncApp.Client.Services
{
    public interface IFinancialService
    {
        Task<HttpResult<List<TransactionDto>>> GetTransactionsAsync(int? userId = null);
        Task<HttpResult<TransactionDto>> GetTransactionByIdAsync(int id);
        Task<HttpResult<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request);
        Task<HttpResult> UpdateTransactionAsync(int id, UpdateTransactionRequest request);
        Task<HttpResult> DeleteTransactionAsync(int id);

        Task<HttpResult<List<CategoryDto>>> GetCategoriesAsync(int? userId = null);
        Task<HttpResult<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<HttpResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<HttpResult> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<HttpResult> DeleteCategoryAsync(int id);
    }
}
