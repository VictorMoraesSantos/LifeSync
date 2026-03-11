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

            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));

            Routing.RegisterRoute("taskdetail", typeof(TaskItemDetailPage));
            Routing.RegisterRoute("tasklabels", typeof(TaskLabelPage));
            Routing.RegisterRoute("ManageTaskItemModal", typeof(ManageTaskItemModal));
            Routing.RegisterRoute("ManageTaskLabelModal", typeof(ManageTaskLabelModal));
            Routing.RegisterRoute("FilterTaskItemPopup", typeof(FilterTaskItemPopup));
            Routing.RegisterRoute("CategoriesPage", typeof(CategoriesPage));
            Routing.RegisterRoute("TransactionListPage", typeof(TransactionListPage));
            Routing.RegisterRoute("ManageTransactionModal", typeof(ManageTransactionModal));
            Routing.RegisterRoute("ManageCategoryModal", typeof(ManageCategoryModal));
            Routing.RegisterRoute("TransactionDetailModal", typeof(TransactionDetailModal));
            Routing.RegisterRoute("FilterTransactionModal", typeof(FilterTransactionModal));
            Routing.RegisterRoute("ManageMealModal", typeof(ManageMealModal));
            Routing.RegisterRoute("MealDetailPage", typeof(MealDetailPage));
            Routing.RegisterRoute("ManageLiquidModal", typeof(ManageLiquidModal));
            Routing.RegisterRoute("DiaryDetailPage", typeof(DiaryDetailPage));
            Routing.RegisterRoute("FoodSearchPage", typeof(FoodSearchPage));
            Routing.RegisterRoute("EditMealFoodModal", typeof(EditMealFoodModal));
            Routing.RegisterRoute("DailyProgressPage", typeof(DailyProgressPage));
            Routing.RegisterRoute("DiaryHistoryPage", typeof(DiaryHistoryPage));
            Routing.RegisterRoute("CreateDiaryModal", typeof(CreateDiaryModal));
            Routing.RegisterRoute("ChangeNameModal", typeof(ChangeNameModal));
            Routing.RegisterRoute("ChangeEmailModal", typeof(ChangeEmailModal));
            Routing.RegisterRoute("ChangePasswordModal", typeof(ChangePasswordModal));
        }
    }
}
