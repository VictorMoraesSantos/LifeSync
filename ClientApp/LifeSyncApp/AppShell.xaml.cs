using LifeSyncApp.Views.Financial;

namespace LifeSyncApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rotas de navegação
            Routing.RegisterRoute("create-transaction", typeof(ManageTransactionPage));
            Routing.RegisterRoute("edit-transaction", typeof(ManageTransactionPage));
        }
    }
}
