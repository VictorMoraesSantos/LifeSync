using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.Contracts;
using Nutrition.Application.DTOs.Food;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ILogger<Food> _logger;

        public FoodService(IFoodRepository foodRepository, ILogger<Food> logger)
        {
            _foodRepository = foodRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<FoodDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _foodRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(FoodMapper.ToDTO)
                    .AsQueryable();
                var count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting Food entities");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _foodRepository.GetById(id, cancellationToken);
                if (entity == null) return Result.Failure<bool>(FoodErrors.NotFound(id));

                await _foodRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Deleted Food entity with ID {Id}", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Food entity with ID {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<FoodDTO?>>> FindAsync(Expression<Func<FoodDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<FoodDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<(IEnumerable<FoodDTO> Items, PaginationData Pagination)>> GetByFilterAsync(FoodQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<FoodDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<(IEnumerable<FoodDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
