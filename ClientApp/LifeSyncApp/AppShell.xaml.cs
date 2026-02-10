using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.TaskManager.TaskLabel;
using LifeSyncApp.Views.Financial;

namespace LifeSyncApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("taskdetail", typeof(TaskItemDetailPage));
            Routing.RegisterRoute("tasklabels", typeof(TaskLabelPage));
            Routing.RegisterRoute("CategoriesPage", typeof(CategoriesPage));
            Routing.RegisterRoute("TransactionListPage", typeof(TransactionListPage));
            Routing.RegisterRoute("ManageTransactionModal", typeof(ManageTransactionModal));
            Routing.RegisterRoute("ManageCategoryModal", typeof(ManageCategoryModal));
        }
    }
}
