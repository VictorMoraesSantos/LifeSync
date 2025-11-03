namespace LifeSyncApp.Client.Models.Dashboard
{
    public class TaskManagerDashboardDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TotalLabels { get; set; }
        public List<TaskStatusBreakdown> StatusBreakdown { get; set; } = new();
        public List<TaskPriorityBreakdown> PriorityBreakdown { get; set; } = new();
        public List<TaskTrend> RecentTasks { get; set; } = new();
    }

    public class TaskStatusBreakdown
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class TaskPriorityBreakdown
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class TaskTrend
    {
        public DateTime Date { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
    }

    public class NutritionDashboardDto
    {
        public int TotalDiaries { get; set; }
        public int TotalMeals { get; set; }
        public int TotalLiquids { get; set; }
        public double AverageCaloriesPerDay { get; set; }
        public double AverageLiquidsPerDay { get; set; }
        public List<NutritionTrend> DailyCaloriesTrend { get; set; } = new();
        public List<MealTypeBreakdown> MealBreakdown { get; set; } = new();
    }

    public class NutritionTrend
    {
        public DateTime Date { get; set; }
        public int Calories { get; set; }
        public int Liquids { get; set; }
    }

    public class MealTypeBreakdown
    {
        public string MealType { get; set; } = string.Empty;
        public int Count { get; set; }
        public int TotalCalories { get; set; }
    }

    public class FinancialDashboardDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalCategories { get; set; }
        public List<TransactionTrend> MonthlyTrend { get; set; } = new();
        public List<CategoryBreakdown> CategoryBreakdown { get; set; } = new();
        public List<PaymentMethodBreakdown> PaymentMethodBreakdown { get; set; } = new();
    }

    public class TransactionTrend
    {
        public string Month { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }

    public class CategoryBreakdown
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public double Percentage { get; set; }
    }

    public class PaymentMethodBreakdown
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class GymDashboardDto
    {
        public int TotalSessions { get; set; }
        public int TotalRoutines { get; set; }
        public int TotalExercises { get; set; }
        public double AverageSessionDuration { get; set; }
        public int SessionsThisMonth { get; set; }
        public List<SessionTrend> WeeklySessions { get; set; } = new();
        public List<RoutineUsage> RoutineUsageStats { get; set; } = new();
        public List<ExerciseFrequency> ExerciseFrequency { get; set; } = new();
    }

    public class SessionTrend
    {
        public string Week { get; set; } = string.Empty;
        public int Sessions { get; set; }
        public double TotalDuration { get; set; }
    }

    public class RoutineUsage
    {
        public string RoutineName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public double AverageDuration { get; set; }
    }

    public class ExerciseFrequency
    {
        public string ExerciseName { get; set; } = string.Empty;
        public int TimesPerformed { get; set; }
    }
}
