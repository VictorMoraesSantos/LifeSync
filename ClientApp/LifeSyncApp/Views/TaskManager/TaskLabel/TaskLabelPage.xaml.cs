using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private readonly TaskLabelViewModel _viewModel;
    private bool _isLoaded;

    public TaskLabelPage(TaskLabelViewModel taskLabelViewModel)
    {
        InitializeComponent();
        _viewModel = taskLabelViewModel;
        BindingContext = taskLabelViewModel;

        // Pré-carregar dados em background durante a construção da página
        _ = Task.Run(async () =>
        {
            if (!_isLoaded)
            {
                await _viewModel.LoadLabelsAsync();
                _isLoaded = true;
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Se ainda não carregou, aguardar
        if (!_isLoaded && !_viewModel.IsBusy)
        {
            Task.Run(async () =>
            {
                await _viewModel.LoadLabelsAsync();
                _isLoaded = true;
            });
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Limpar seleção ao sair
        _viewModel.SelectedLabel = null;
    }
}
