using LifeSyncApp.Constants;
using LifeSyncApp.Views.Auth;
using LifeSyncApp.Views.Financial;
using LifeSyncApp.Views.Nutrition;
using LifeSyncApp.Views.Profile;
using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.TaskManager.TaskLabel;

namespace LifeSyncApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(AppRoutes.LoginPage, typeof(LoginPage));
            Routing.RegisterRoute(AppRoutes.RegisterPage, typeof(RegisterPage));

            Routing.RegisterRoute(AppRoutes.TaskDetail, typeof(TaskItemDetailPage));
            Routing.RegisterRoute(AppRoutes.TaskLabels, typeof(TaskLabelPage));
            Routing.RegisterRoute(AppRoutes.ManageTaskItemModal, typeof(ManageTaskItemModal));
            Routing.RegisterRoute(AppRoutes.ManageTaskLabelModal, typeof(ManageTaskLabelModal));
            Routing.RegisterRoute(AppRoutes.FilterTaskItemPopup, typeof(FilterTaskItemPopup));
            Routing.RegisterRoute(AppRoutes.CategoriesPage, typeof(CategoriesPage));
            Routing.RegisterRoute(AppRoutes.TransactionListPage, typeof(TransactionListPage));
            Routing.RegisterRoute(AppRoutes.ManageTransactionModal, typeof(ManageTransactionModal));
            Routing.RegisterRoute(AppRoutes.ManageCategoryModal, typeof(ManageCategoryModal));
            Routing.RegisterRoute(AppRoutes.TransactionDetailModal, typeof(TransactionDetailModal));
            Routing.RegisterRoute(AppRoutes.FilterTransactionModal, typeof(FilterTransactionModal));
            Routing.RegisterRoute(AppRoutes.ManageMealModal, typeof(ManageMealModal));
            Routing.RegisterRoute(AppRoutes.MealDetailPage, typeof(MealDetailPage));
            Routing.RegisterRoute(AppRoutes.ManageLiquidModal, typeof(ManageLiquidModal));
            Routing.RegisterRoute(AppRoutes.DiaryDetailPage, typeof(DiaryDetailPage));
            Routing.RegisterRoute(AppRoutes.FoodSearchPage, typeof(FoodSearchPage));
            Routing.RegisterRoute(AppRoutes.EditMealFoodModal, typeof(EditMealFoodModal));
            Routing.RegisterRoute(AppRoutes.DailyProgressPage, typeof(DailyProgressPage));
            Routing.RegisterRoute(AppRoutes.DiaryHistoryPage, typeof(DiaryHistoryPage));
            Routing.RegisterRoute(AppRoutes.CreateDiaryModal, typeof(CreateDiaryModal));
            Routing.RegisterRoute(AppRoutes.ChangeNameModal, typeof(ChangeNameModal));
            Routing.RegisterRoute(AppRoutes.ChangeEmailModal, typeof(ChangeEmailModal));
            Routing.RegisterRoute(AppRoutes.ChangePasswordModal, typeof(ChangePasswordModal));
        }
    }
}
