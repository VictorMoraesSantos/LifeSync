using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class DailyProgressMapper
    {
        public static DailyProgress ToEntity(CreateDailyProgressDTO dto)
        {
            DailyProgress entity = new(dto.UserId, dto.Date, (int)dto.CaloriesConsumed, (int)dto.LiquidsConsumedMl);
            return entity;
        }

        public static DailyProgressDTO ToDTO(DailyProgress entity)
        {
            DailyGoalDTO goalDTO = new(entity.Goal.Calories, entity.Goal.QuantityMl);
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