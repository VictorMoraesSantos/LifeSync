using LifeSyncApp.Views.Financial;
using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.TaskManager.TaskLabel;

namespace LifeSyncApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

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
        }
    }
}
