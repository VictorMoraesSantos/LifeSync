using Nutrition.Application.DTOs.Diaries;
using Nutrition.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nutrition.Application.Mapping
{
    public static class DiaryMapper
    {
        public static DiaryDTO ToDTO(this Diary entity)
        {
            DiaryDTO dto = new(
                entity.Id,
                entity.UserId,
                entity.Date,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.TotalCalories,
                entity.Meals.Select(m => m.ToDTO()).ToList(),
                entity.Liquids.Select(m => m.ToDTO()).ToList());

            return dto;
        }

        public static Diary ToEntity(this CreateDiaryDTO dto)
        {
            Diary entity = new(
                dto.userId,
                dto.date);

            return entity;
        }
    }
}
