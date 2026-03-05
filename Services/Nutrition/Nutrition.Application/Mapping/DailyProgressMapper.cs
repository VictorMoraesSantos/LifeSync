using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class DailyProgressMapper
    {
        public static DailyProgress ToEntity(CreateDailyProgressDTO dto)
        {
            DailyProgress entity = new(dto.UserId, dto.Date, dto.CaloriesConsumed ?? 0, dto.LiquidsConsumedMl ?? 0);
            return entity;
        }

        public static DailyProgressDTO ToDTO(DailyProgress entity)
        {
            DailyGoalDTO? goalDTO = entity.Goal != null
                ? new(entity.Goal.Calories, entity.Goal.QuantityMl)
                : null;
            DailyProgressDTO dailyProgress = new DailyProgressDTO(
                entity.Id,
                entity.UserId,
                entity.Date,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.CaloriesConsumed,
                entity.LiquidsConsumedMl,
                goalDTO);
            return dailyProgress;
        }
    }
}