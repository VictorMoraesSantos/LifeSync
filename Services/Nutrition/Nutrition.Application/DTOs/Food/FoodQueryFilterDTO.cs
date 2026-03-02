using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Food
{
    public record FoodQueryFilterDTO(
        int Id,
        string Name,
        int CaloriesEquals,
        int CaloriesGreaterThan,
        int CaloriesLessThan,
        decimal ProteinEquals,
        decimal ProteinGreaterThan,
        decimal ProteinLessThan,
        decimal LipidsEquals,
        decimal LipidsGreaterThan,
        decimal LipidsLessThan,
        decimal CarbohydratesEquals,
        decimal CarbohydratesGreaterThan,
        decimal CarbohydratesLessThan,
        decimal CalciumEquals,
        decimal CalciumGreaterThan,
        decimal CalciumLessThan,
        decimal MagnesiumEquals,
        decimal MagnesiumGreaterThan,
        decimal MagnesiumLessThan,
        decimal IronEquals,
        decimal IronGreaterThan,
        decimal IronLessThan,
        decimal SodiumEquals,
        decimal SodiumGreaterThan,
        decimal SodiumLessThan,
        decimal PotassiumEquals,
        decimal PotassiumGreaterThan,
        decimal PotassiumLessThan,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
