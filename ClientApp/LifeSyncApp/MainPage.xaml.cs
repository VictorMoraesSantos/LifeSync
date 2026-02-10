using LifeSyncApp.Views.Financial;
using LifeSyncApp.Views.Academic;
using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.Nutrition;

namespace LifeSyncApp;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;
    private ContentPage? _currentPage;
    private int _currentTabIndex = -1;

    public MainPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        System.Diagnostics.Debug.WriteLine("MainPage Constructor - Starting");

        try
        {
            // Inicializar com a primeira tab (Financeiro)
            LoadPage(0);
            System.Diagnostics.Debug.WriteLine("MainPage Constructor - LoadPage(0) completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPage Constructor Error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Inicializar dados de forma não bloqueante
        InitializeCurrentPageAsync();
    }

    private async void InitializeCurrentPageAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("InitializeCurrentPageAsync - Starting");

            if (_currentPage?.BindingContext is ViewModels.Financial.FinancialViewModel financialVm)
            {
                System.Diagnostics.Debug.WriteLine("Calling FinancialViewModel InitializeAsync");
                await financialVm.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("FinancialViewModel InitializeAsync completed");
            }
            else if (_currentPage?.BindingContext is ViewModels.TaskManager.TaskItemsViewModel taskVm)
            {
                System.Diagnostics.Debug.WriteLine("Calling TaskItemsViewModel LoadTasksAsync");
                // TaskItemsViewModel não tem InitializeAsync, mas tem LoadTasksAsync
                // Vamos deixar ele carregar no OnAppearing dele
            }

            System.Diagnostics.Debug.WriteLine("InitializeCurrentPageAsync - Completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void OnTabSelected(object sender, int tabIndex)
    {
        System.Diagnostics.Debug.WriteLine($"OnTabSelected called with index: {tabIndex}");

        // Evitar recarregar a mesma tab
        if (_currentTabIndex == tabIndex && _currentPage != null)
        {
            System.Diagnostics.Debug.WriteLine($"Same tab selected, skipping reload");
            return;
        }

        LoadPage(tabIndex);
    }

    private void LoadPage(int tabIndex)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"LoadPage called for tab {tabIndex}");

            // Criar nova página apenas se necessário
            ContentPage page = tabIndex switch
            {
                0 => _serviceProvider.GetRequiredService<FinancialPage>(),
                1 => _serviceProvider.GetRequiredService<AcademicPage>(),
                2 => _serviceProvider.GetRequiredService<TaskItemPage>(),
                3 => _serviceProvider.GetRequiredService<NutritionPage>(),
                _ => throw new ArgumentException("Invalid tab index")
            };

            System.Diagnostics.Debug.WriteLine($"Page created: {page.GetType().Name}");

            // Extrair o conteúdo
            var content = page.Content;

            if (content == null)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: Page content is null!");
                return;
            }

            // IMPORTANTE: Transferir o BindingContext do page para o content
            if (page.BindingContext != null)
            {
                content.BindingContext = page.BindingContext;
                System.Diagnostics.Debug.WriteLine($"BindingContext transferred: {page.BindingContext.GetType().Name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("WARNING: Page BindingContext is null!");
            }

            // Mostrar o conteúdo
            ContentArea.Content = content;

            // Manter referência à página para manter o BindingContext ativo
            _currentPage = page;
            _currentTabIndex = tabIndex;

            TabBar.SelectedTab = tabIndex;

            System.Diagnostics.Debug.WriteLine($"Tab {tabIndex} loaded successfully");

            // Simular OnAppearing para páginas que não são Financial
            if (tabIndex == 2 && page.BindingContext is ViewModels.TaskManager.TaskItemsViewModel taskVm)
            {
                System.Diagnostics.Debug.WriteLine("Loading tasks for TaskItemPage...");
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await taskVm.LoadTasksAsync();
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading page: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
