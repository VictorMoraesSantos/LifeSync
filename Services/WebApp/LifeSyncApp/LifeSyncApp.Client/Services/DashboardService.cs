using LifeSyncApp.Client.Models.Dashboard;
using LifeSyncApp.Client.Models.Financial;
using LifeSyncApp.Client.Models.Gym;
using LifeSyncApp.Client.Models.TaskManager;

namespace LifeSyncApp.Client.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITaskManagerService _taskManagerService;
        private readonly INutritionService _nutritionService;
        private readonly IFinancialService _financialService;
        private readonly IGymService _gymService;

        public DashboardService(
            ITaskManagerService taskManagerService,
            INutritionService nutritionService,
            IFinancialService financialService,
            IGymService gymService)
        {
            _taskManagerService = taskManagerService;
            _nutritionService = nutritionService;
            _financialService = financialService;
            _gymService = gymService;
        }

        public async Task<TaskManagerDashboardDto> GetTaskManagerDashboardAsync(int userId)
        {
            var tasksResult = await _taskManagerService.GetTaskItemsAsync(userId);
            var labelsResult = await _taskManagerService.GetTaskLabelsAsync(userId);

            if (!tasksResult.Success || tasksResult.Data == null)
                return new TaskManagerDashboardDto();

            var tasks = tasksResult.Data;
            var labels = labelsResult.Success ? labelsResult.Data ?? new List<TaskLabelDto>() : new List<TaskLabelDto>();

            var dashboard = new TaskManagerDashboardDto
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == 3),
                PendingTasks = tasks.Count(t => t.Status == 1),
                InProgressTasks = tasks.Count(t => t.Status == 2),
                TotalLabels = labels.Count
            };

            // Status breakdown
            dashboard.StatusBreakdown = new List<TaskStatusBreakdown>
            {
                new TaskStatusBreakdown { Status = "Pendente", Count = dashboard.PendingTasks, Percentage = dashboard.TotalTasks > 0 ? (double)dashboard.PendingTasks / dashboard.TotalTasks * 100 : 0 },
                new TaskStatusBreakdown { Status = "Em Progresso", Count = dashboard.InProgressTasks, Percentage = dashboard.TotalTasks > 0 ? (double)dashboard.InProgressTasks / dashboard.TotalTasks * 100 : 0 },
                new TaskStatusBreakdown { Status = "Completada", Count = dashboard.CompletedTasks, Percentage = dashboard.TotalTasks > 0 ? (double)dashboard.CompletedTasks / dashboard.TotalTasks * 100 : 0 },
                new TaskStatusBreakdown { Status = "Cancelada", Count = tasks.Count(t => t.Status == 4), Percentage = dashboard.TotalTasks > 0 ? (double)tasks.Count(t => t.Status == 4) / dashboard.TotalTasks * 100 : 0 }
            };

            // Priority breakdown
            dashboard.PriorityBreakdown = new List<TaskPriorityBreakdown>
            {
                new TaskPriorityBreakdown { Priority = "Baixa", Count = tasks.Count(t => t.Priority == 1), Percentage = dashboard.TotalTasks > 0 ? (double)tasks.Count(t => t.Priority == 1) / dashboard.TotalTasks * 100 : 0 },
                new TaskPriorityBreakdown { Priority = "Média", Count = tasks.Count(t => t.Priority == 2), Percentage = dashboard.TotalTasks > 0 ? (double)tasks.Count(t => t.Priority == 2) / dashboard.TotalTasks * 100 : 0 },
                new TaskPriorityBreakdown { Priority = "Alta", Count = tasks.Count(t => t.Priority == 3), Percentage = dashboard.TotalTasks > 0 ? (double)tasks.Count(t => t.Priority == 3) / dashboard.TotalTasks * 100 : 0 },
                new TaskPriorityBreakdown { Priority = "Urgente", Count = tasks.Count(t => t.Priority == 4), Percentage = dashboard.TotalTasks > 0 ? (double)tasks.Count(t => t.Priority == 4) / dashboard.TotalTasks * 100 : 0 }
            };

            // Recent tasks trend (last 7 days)
            var last7Days = tasks.Where(t => t.CreatedAt >= DateTime.Now.AddDays(-7))
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new TaskTrend
                {
                    Date = g.Key,
                    Created = g.Count(),
                    Completed = g.Count(t => t.Status == 3)
                }).ToList();

            dashboard.RecentTasks = last7Days;

            return dashboard;
        }

        public async Task<NutritionDashboardDto> GetNutritionDashboardAsync(int userId)
        {
            var diariesResult = await _nutritionService.GetDiariesAsync(userId);

            if (!diariesResult.Success || diariesResult.Data == null)
                return new NutritionDashboardDto();

            var diaries = diariesResult.Data;

            var dashboard = new NutritionDashboardDto
            {
                TotalDiaries = diaries.Count,
                TotalMeals = diaries.Sum(d => d.Meals.Count),
                TotalLiquids = diaries.Sum(d => d.Liquids.Count),
                AverageCaloriesPerDay = diaries.Any() ? diaries.Average(d => d.TotalCalories) : 0,
                AverageLiquidsPerDay = diaries.Any() ? diaries.Average(d => d.Liquids.Sum(l => l.QuantityMl)) : 0
            };

            // Daily calories trend (last 14 days)
            var last14Days = diaries
                .Where(d => d.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-14)))
                .OrderBy(d => d.Date)
                .Select(d => new NutritionTrend
                {
                    Date = d.Date.ToDateTime(TimeOnly.MinValue),
                    Calories = d.TotalCalories,
                    Liquids = d.Liquids.Sum(l => l.QuantityMl)
                }).ToList();

            dashboard.DailyCaloriesTrend = last14Days;

            // Meal breakdown
            var allMeals = diaries.SelectMany(d => d.Meals).ToList();
            dashboard.MealBreakdown = allMeals
                .GroupBy(m => m.Name)
                .Select(g => new MealTypeBreakdown
                {
                    MealType = g.Key,
                    Count = g.Count(),
                    TotalCalories = g.Sum(m => m.TotalCalories)
                })
                .OrderByDescending(m => m.TotalCalories)
                .Take(10)
                .ToList();

            return dashboard;
        }

        public async Task<FinancialDashboardDto> GetFinancialDashboardAsync(int userId)
        {
            var transactionsResult = await _financialService.GetTransactionsAsync(userId);
            var categoriesResult = await _financialService.GetCategoriesAsync(userId);

            if (!transactionsResult.Success || transactionsResult.Data == null)
                return new FinancialDashboardDto();

            var transactions = transactionsResult.Data;
            var categories = categoriesResult.Success ? categoriesResult.Data ?? new List<CategoryDto>() : new List<CategoryDto>();

            var incomes = transactions.Where(t => t.TransactionType == 1).ToList();
            var expenses = transactions.Where(t => t.TransactionType == 2).ToList();

            var dashboard = new FinancialDashboardDto
            {
                TotalIncome = incomes.Sum(t => t.Amount),
                TotalExpenses = expenses.Sum(t => t.Amount),
                NetBalance = incomes.Sum(t => t.Amount) - expenses.Sum(t => t.Amount),
                TotalTransactions = transactions.Count,
                TotalCategories = categories.Count
            };

            // Monthly trend (last 6 months)
            var last6Months = transactions
                .Where(t => t.TransactionDate >= DateTime.Now.AddMonths(-6))
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new TransactionTrend
                {
                    Month = $"{g.Key.Year}/{g.Key.Month:00}",
                    Income = g.Where(t => t.TransactionType == 1).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.TransactionType == 2).Sum(t => t.Amount)
                })
                .OrderBy(t => t.Month)
                .ToList();

            dashboard.MonthlyTrend = last6Months;

            // Category breakdown
            var categoryTransactions = transactions.Where(t => t.Category != null).ToList();
            var totalCategoryAmount = categoryTransactions.Sum(t => t.Amount);

            dashboard.CategoryBreakdown = categoryTransactions
                .GroupBy(t => t.Category!.Name)
                .Select(g => new CategoryBreakdown
                {
                    CategoryName = g.Key,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count(),
                    Percentage = totalCategoryAmount > 0 ? (double)(g.Sum(t => t.Amount) / totalCategoryAmount * 100) : 0
                })
                .OrderByDescending(c => c.Amount)
                .Take(10)
                .ToList();

            // Payment method breakdown
            dashboard.PaymentMethodBreakdown = transactions
                .GroupBy(t => t.PaymentMethod)
                .Select(g => new PaymentMethodBreakdown
                {
                    PaymentMethod = GetPaymentMethodName(g.Key),
                    Amount = g.Sum(t => t.Amount),
                    Count = g.Count()
                })
                .ToList();

            return dashboard;
        }

        public async Task<GymDashboardDto> GetGymDashboardAsync(int userId)
        {
            var sessionsResult = await _gymService.GetTrainingSessionsAsync();
            var routinesResult = await _gymService.GetRoutinesAsync();
            var exercisesResult = await _gymService.GetExercisesAsync();

            if (!sessionsResult.Success || sessionsResult.Data == null)
                return new GymDashboardDto();

            var sessions = sessionsResult.Data.Where(s => s.UserId == userId).ToList();
            var routines = routinesResult.Success ? routinesResult.Data ?? new List<RoutineDto>() : new List<RoutineDto>();
            var exercises = exercisesResult.Success ? exercisesResult.Data ?? new List<ExerciseDto>() : new List<ExerciseDto>();

            var sessionsThisMonth = sessions.Count(s => s.StartTime.Month == DateTime.Now.Month && s.StartTime.Year == DateTime.Now.Year);

            var dashboard = new GymDashboardDto
            {
                TotalSessions = sessions.Count,
                TotalRoutines = routines.Count,
                TotalExercises = exercises.Count,
                SessionsThisMonth = sessionsThisMonth,
                AverageSessionDuration = sessions
                    .Where(s => s.EndTime.HasValue)
                    .Select(s => (s.EndTime!.Value - s.StartTime).TotalMinutes)
                    .DefaultIfEmpty(0)
                    .Average()
            };

            // Weekly sessions trend (last 4 weeks)
            var last4Weeks = sessions
                .Where(s => s.StartTime >= DateTime.Now.AddDays(-28))
                .GroupBy(s => GetWeekLabel(s.StartTime))
                .Select(g => new SessionTrend
                {
                    Week = g.Key,
                    Sessions = g.Count(),
                    TotalDuration = g
                        .Where(s => s.EndTime.HasValue)
                        .Sum(s => (s.EndTime!.Value - s.StartTime).TotalMinutes)
                })
                .ToList();

            dashboard.WeeklySessions = last4Weeks;

            // Routine usage stats
            dashboard.RoutineUsageStats = sessions
                .Where(s => s.EndTime.HasValue)
                .GroupBy(s => s.RoutineId)
                .Select(g => new RoutineUsage
                {
                    RoutineName = routines.FirstOrDefault(r => r.Id == g.Key)?.Name ?? $"Rotina #{g.Key}",
                    UsageCount = g.Count(),
                    AverageDuration = g.Average(s => (s.EndTime!.Value - s.StartTime).TotalMinutes)
                })
                .OrderByDescending(r => r.UsageCount)
                .Take(10)
                .ToList();

            // Exercise frequency (would need completed exercises data)
            dashboard.ExerciseFrequency = exercises
                .Take(10)
                .Select(e => new ExerciseFrequency
                {
                    ExerciseName = e.Name,
                    TimesPerformed = 0 // This would require completed exercises data
                })
                .ToList();

            return dashboard;
        }

        private string GetPaymentMethodName(int method)
        {
            return method switch
            {
                1 => "Dinheiro",
                2 => "Cartão de Crédito",
                3 => "Cartão de Débito",
                4 => "Transferência",
                5 => "Carteira Digital",
                _ => "Desconhecido"
            };
        }

        private string GetWeekLabel(DateTime date)
        {
            var startOfWeek = date.AddDays(-(int)date.DayOfWeek);
            return $"Semana {startOfWeek:dd/MM}";
        }
    }
}
