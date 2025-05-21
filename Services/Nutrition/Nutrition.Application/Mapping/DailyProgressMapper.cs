using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Domain.Entities;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Mapping
{
    public static class DailyProgressMapper
    {
        public static DailyProgress ToEntity(CreateDailyProgressDTO dto)
        {
            DailyGoal goal = new DailyGoal(dto.Goal.Calories, dto.Goal.QuantityMl);
            DailyProgress entity = new DailyProgress(dto.UserId, dto.Date, (int)dto.CaloriesConsumed, (int)dto.LiquidsConsumedMl, goal);

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

        public static void UpdateEntity(DailyProgress entity, UpdateDailyProgressDTO dto)
        {
            if (entity == null || dto == null) return;

            entity.SetConsumed((int)dto.CaloriesConsumed, (int)dto.LiquidsConsumedMl);

            if (dto.Goal != null)
            {
                var goal = new DailyGoal(dto.Goal.Calories, dto.Goal.QuantityMl);
                entity.SetGoal(goal);
            }
        }
    }
}