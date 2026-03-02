using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class FoodQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? Name { get; private set; }
        public decimal? CaloriesEquals { get; private set; }
        public decimal? CaloriesGreaterThan { get; private set; }
        public decimal? CaloriesLessThan { get; private set; }
        public decimal? ProteinEquals { get; private set; }
        public decimal? ProteinGreaterThan { get; private set; }
        public decimal? ProteinLessThan { get; private set; }
        public decimal? LipidsEquals { get; private set; }
        public decimal? LipidsGreatherThan { get; private set; }
        public decimal? LipidsLessThan { get; private set; }
        public decimal? CarbohydratesEquals { get; set; }
        public decimal? CarbohydratesGreaterThan { get; set; }
        public decimal? CarbohydratesLessThan { get; set; }
        public decimal? CalciumEquals { get; private set; }
        public decimal? CalciumGreaterThan { get; private set; }
        public decimal? CalciumLessThan { get; private set; }
        public decimal? MagnesiumEquals { get; private set; }
        public decimal? MagnesiumGreaterThan { get; private set; }
        public decimal? MagnesiumLessThan { get; private set; }
        public decimal? IronEquals { get; private set; }
        public decimal? IronGreaterThan { get; private set; }
        public decimal? IronLessThan { get; private set; }
        public decimal? SodiumEquals { get; private set; }
        public decimal? SodiumGreaterThan { get; private set; }
        public decimal? SodiumLessThan { get; private set; }
        public decimal? PotassiumEquals { get; private set; }
        public decimal? PotassiumGreaterThan { get; private set; }
        public decimal? PotassiumLessThan { get; private set; }

        public FoodQueryFilter(
           int? id = null,
           string? name = null,
           decimal? caloriesEquals = null,
           decimal? caloriesGreaterThan = null,
           decimal? caloriesLessThan = null,
           decimal? proteinEquals = null,
           decimal? proteinGreaterThan = null,
           decimal? proteinLessThan = null,
           decimal? lipidsEquals = null,
           decimal? lipidsGreatherThan = null,
           decimal? lipidsLessThan = null,
           decimal? carbohydratesEquals = null,
           decimal? carbohydratesGreaterThan = null,
           decimal? carbohydratesLessThan = null,
           decimal? calciumEquals = null,
           decimal? calciumGreaterThan = null,
           decimal? calciumLessThan = null,
           decimal? magnesiumEquals = null,
           decimal? magnesiumGreaterThan = null,
           decimal? magnesiumLessThan = null,
           decimal? ironEquals = null,
           decimal? ironGreaterThan = null,
           decimal? ironLessThan = null,
           decimal? sodiumEquals = null,
           decimal? sodiumGreaterThan = null,
           decimal? sodiumLessThan = null,
           decimal? potassiumEquals = null,
           decimal? potassiumGreaterThan = null,
           decimal? potassiumLessThan = null,
           DateOnly? createdAt = null,
           DateOnly? updatedAt = null,
           bool? isDeleted = null,
           string? sortBy = null,
           bool? sortDesc = null,
           int? page = null,
           int? pageSize = null)
        {
            Id = id;
            Name = name;
            CaloriesEquals = caloriesEquals;
            CaloriesGreaterThan = caloriesGreaterThan;
            CaloriesLessThan = caloriesLessThan;
            ProteinEquals = proteinEquals;
            ProteinGreaterThan = proteinGreaterThan;
            ProteinLessThan = proteinLessThan;
            LipidsEquals = lipidsEquals;
            LipidsGreatherThan = lipidsGreatherThan;
            LipidsLessThan = lipidsLessThan;
            CarbohydratesEquals = carbohydratesEquals;
            CarbohydratesGreaterThan = carbohydratesGreaterThan;
            CarbohydratesLessThan = carbohydratesLessThan;
            CalciumEquals = calciumEquals;
            CalciumGreaterThan = calciumGreaterThan;
            CalciumLessThan = calciumLessThan;
            MagnesiumEquals = magnesiumEquals;
            MagnesiumGreaterThan = magnesiumGreaterThan;
            MagnesiumLessThan = magnesiumLessThan;
            IronEquals = ironEquals;
            IronGreaterThan = ironGreaterThan;
            IronLessThan = ironLessThan;
            SodiumEquals = sodiumEquals;
            SodiumGreaterThan = sodiumGreaterThan;
            SodiumLessThan = sodiumLessThan;
            PotassiumEquals = potassiumEquals;
            PotassiumGreaterThan = potassiumGreaterThan;
            PotassiumLessThan = potassiumLessThan;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page;
            PageSize = pageSize;
        }
    }
}
