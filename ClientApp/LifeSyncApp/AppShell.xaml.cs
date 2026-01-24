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
        }
    }
}
