using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetByFilter
{
    public record GetCategoriesByFilterQuery(CategoryFilterDTO Filter) : IQuery<GetCategoriesByFilterResult>;
    public record GetCategoriesByFilterResult(IEnumerable<CategoryDTO> Items, PaginationData Pagination);
}
