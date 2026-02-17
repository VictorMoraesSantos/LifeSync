using LifeSyncApp.Views.Academic;
using LifeSyncApp.Views.Financial;
using LifeSyncApp.Views.Nutrition;
using LifeSyncApp.Views.TaskManager.TaskItem;

namespace LifeSyncApp;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;
    private ContentPage? _currentPage;
    private int _currentTabIndex = -1;

    public MainPage(IServiceProvider serviceProvider)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔵 MainPage: Construtor iniciado");
            InitializeComponent();
            _serviceProvider = serviceProvider;
            System.Diagnostics.Debug.WriteLine("✅ MainPage: InitializeComponent OK");

            // Inicializar com a primeira tab (Financeiro)
            LoadPage(0);
            System.Diagnostics.Debug.WriteLine("✅ MainPage: Construtor completo");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ ERRO FATAL MainPage Construtor:");
            System.Diagnostics.Debug.WriteLine($"  Mensagem: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"  InnerException: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        InitializeCurrentPageAsync();
    }

    private async void InitializeCurrentPageAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔵 InitializeCurrentPageAsync iniciado");

            if (_currentPage?.BindingContext is ViewModels.Financial.FinancialViewModel financialVm)
            {
                System.Diagnostics.Debug.WriteLine("🔵 Chamando FinancialViewModel.InitializeAsync");
                await financialVm.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("✅ FinancialViewModel.InitializeAsync OK");
            }
            else if (_currentPage?.BindingContext is ViewModels.TaskManager.TaskItemsViewModel taskVm)
            {
                System.Diagnostics.Debug.WriteLine("🔵 TaskItemsViewModel detectado");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ ERRO InitializeCurrentPageAsync:");
            System.Diagnostics.Debug.WriteLine($"  Mensagem: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
        }
    }

    private void OnTabSelected(object sender, int tabIndex)
    {
        System.Diagnostics.Debug.WriteLine($"🔵 Tab selecionada: {tabIndex}");

        // Evitar recarregar a mesma tab
        if (_currentTabIndex == tabIndex && _currentPage != null)
        {
            System.Diagnostics.Debug.WriteLine($"🟡 Mesma tab, pulando reload");
            return;
        }

        LoadPage(tabIndex);
    }

    private void LoadPage(int tabIndex)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔵 LoadPage iniciado para tab {tabIndex}");

            ContentPage page;

            // Criar página com try-catch individual
            try
            {
                page = tabIndex switch
                {
                    0 => _serviceProvider.GetRequiredService<FinancialPage>(),
                    1 => _serviceProvider.GetRequiredService<AcademicPage>(),
                    2 => _serviceProvider.GetRequiredService<TaskItemPage>(),
                    3 => _serviceProvider.GetRequiredService<NutritionPage>(),
                    _ => throw new ArgumentException($"Tab index inválido: {tabIndex}")
                };
                System.Diagnostics.Debug.WriteLine($"✅ Página criada: {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO ao criar página tab {tabIndex}:");
                System.Diagnostics.Debug.WriteLine($"  Mensagem: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  InnerException: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"  Inner StackTrace: {ex.InnerException.StackTrace}");
                }

                // Mostrar erro visual
                ContentArea.Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(20),
                    Children =
                    {
                        new Label
                        {
                            Text = "❌ Erro ao Carregar Página",
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Red
                        },
                        new Label
                        {
                            Text = ex.Message,
                            FontSize = 14,
                            TextColor = Colors.Black
                        },
                        new Label
                        {
                            Text = ex.StackTrace,
                            FontSize = 10,
                            TextColor = Colors.Gray
                        }
                    }
                };
                return;
            }

            // Extrair conteúdo
            var content = page.Content;
            if (content == null)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ WARNING: Content é null para tab {tabIndex}");
                ContentArea.Content = new Label
                {
                    Text = $"⚠️ Conteúdo da página {tabIndex} está vazio",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                return;
            }

            System.Diagnostics.Debug.WriteLine($"✅ Content extraído: {content.GetType().Name}");

            // Transferir BindingContext
            if (page.BindingContext != null)
            {
                content.BindingContext = page.BindingContext;
                System.Diagnostics.Debug.WriteLine($"✅ BindingContext transferido: {page.BindingContext.GetType().Name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ WARNING: BindingContext é null para tab {tabIndex}");
            }

            // Exibir conteúdo
            ContentArea.Content = content;
            _currentPage = page;
            _currentTabIndex = tabIndex;
            TabBar.SelectedTab = tabIndex;

            System.Diagnostics.Debug.WriteLine($"✅ Tab {tabIndex} carregada com sucesso");

            // Carregar tasks se for TaskManager
            if (tabIndex == 2 && page.BindingContext is ViewModels.TaskManager.TaskItemsViewModel taskVm)
            {
                System.Diagnostics.Debug.WriteLine("🔵 Iniciando load de tasks");
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
                        System.Diagnostics.Debug.WriteLine($"❌ Erro ao carregar tasks: {ex.Message}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ ERRO GERAL LoadPage tab {tabIndex}:");
            System.Diagnostics.Debug.WriteLine($"  Mensagem: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");

            // Mostrar erro visual
            try
            {
                ContentArea.Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(20),
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "❌ Erro Fatal",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Red,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = ex.Message,
                            FontSize = 14,
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Center
                        }
                    }
                };
            }
            catch (Exception displayEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro até para mostrar erro: {displayEx.Message}");
            }
        }
    }
}
