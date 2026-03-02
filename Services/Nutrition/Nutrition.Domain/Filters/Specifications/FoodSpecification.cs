using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class FoodSpecification : Specification<Food, int>
    {
        public FoodSpecification(FoodQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, f => f.Id == filter.Id!.Value);
            AddIf(filter.Name is not null, f => f.Name.Contains(filter.Name!.ToLower()!));
            AddIf(filter.CaloriesEquals.HasValue, f => f.Calories == filter.CaloriesEquals!.Value);
            AddIf(filter.CaloriesGreaterThan.HasValue, f => f.Calories > filter.CaloriesGreaterThan!.Value);
            AddIf(filter.CaloriesLessThan.HasValue, f => f.Calories < filter.CaloriesLessThan!.Value);
            AddIf(filter.ProteinEquals.HasValue, f => f.Protein == filter.ProteinEquals!.Value);
            AddIf(filter.ProteinGreaterThan.HasValue, f => f.Protein > filter.ProteinGreaterThan!.Value);
            AddIf(filter.ProteinLessThan.HasValue, f => f.Protein < filter.ProteinLessThan!.Value);
            AddIf(filter.LipidsEquals.HasValue, f => f.Lipids == filter.LipidsEquals!.Value);
            AddIf(filter.LipidsGreatherThan.HasValue, f => f.Lipids > filter.LipidsGreatherThan!.Value);
            AddIf(filter.LipidsLessThan.HasValue, f => f.Lipids < filter.LipidsLessThan!.Value);
            AddIf(filter.CarbohydratesEquals.HasValue, f => f.Carbohydrates == filter.CarbohydratesEquals!.Value);
            AddIf(filter.CarbohydratesGreaterThan.HasValue, f => f.Carbohydrates > filter.CarbohydratesGreaterThan!.Value);
            AddIf(filter.CarbohydratesLessThan.HasValue, f => f.Carbohydrates < filter.CarbohydratesLessThan!.Value);
            AddIf(filter.CalciumEquals.HasValue, f => f.Calcium == filter.CalciumEquals!.Value);
            AddIf(filter.CalciumGreaterThan.HasValue, f => f.Calcium > filter.CalciumGreaterThan!.Value);
            AddIf(filter.CalciumLessThan.HasValue, f => f.Calcium < filter.CalciumLessThan!.Value);
            AddIf(filter.MagnesiumEquals.HasValue, f => f.Magnesium == filter.MagnesiumEquals!.Value);
            AddIf(filter.MagnesiumGreaterThan.HasValue, f => f.Magnesium > filter.MagnesiumGreaterThan!.Value);
            AddIf(filter.MagnesiumLessThan.HasValue, f => f.Magnesium < filter.MagnesiumLessThan!.Value);
            AddIf(filter.IronEquals.HasValue, f => f.Iron == filter.IronEquals!.Value);
            AddIf(filter.IronGreaterThan.HasValue, f => f.Iron > filter.IronGreaterThan!.Value);
            AddIf(filter.IronLessThan.HasValue, f => f.Iron < filter.IronLessThan!.Value);
            AddIf(filter.SodiumEquals.HasValue, f => f.Sodium == filter.SodiumEquals!.Value);
            AddIf(filter.SodiumGreaterThan.HasValue, f => f.Sodium > filter.SodiumGreaterThan!.Value);
            AddIf(filter.SodiumLessThan.HasValue, f => f.Sodium < filter.SodiumLessThan!.Value);
            AddIf(filter.PotassiumEquals.HasValue, f => f.Potassium == filter.PotassiumEquals!.Value);
            AddIf(filter.PotassiumGreaterThan.HasValue, f => f.Potassium > filter.PotassiumGreaterThan!.Value);
            AddIf(filter.PotassiumLessThan.HasValue, f => f.Potassium < filter.PotassiumLessThan!.Value);
        }
    }
}
