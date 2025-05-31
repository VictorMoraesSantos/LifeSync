using Financial.Application.DTOs.Category;
using MediatR;

namespace Financial.Application.Features.Categories.Queries.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<GetAllCategoriesResult>;
    public record GetAllCategoriesResult(IEnumerable<CategoryDTO> Categories);
}
