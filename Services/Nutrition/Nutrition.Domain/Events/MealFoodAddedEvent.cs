﻿using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealFoodAddedEvent : DomainEvent
    {
        public int DiaryId { get; }
        public int TotalCalories { get; }

        public MealFoodAddedEvent(int diaryId, int totalCalories)
        {
            DiaryId = diaryId;
            TotalCalories = totalCalories;
        }
    }
}