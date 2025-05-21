using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Mapping
{
    public static class DailyGoalMapper
    {
        public static DailyGoal ToEntity(DailyGoalDTO dto)
        {
            DailyGoal entity = new(dto.Calories, dto.QuantityMl);
            return entity;
        }

        public static DailyGoalDTO ToDTO(DailyGoal entity)
        {
            DailyGoalDTO dto = new(entity.Calories, entity.QuantityMl);
            return dto;
        }
    }
}