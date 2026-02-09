using LifeSyncApp.Models.TaskManager;
using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class TaskCard : ContentView
{
    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TaskCard));

    public static readonly BindableProperty StatusToggleCommandProperty =
        BindableProperty.Create(nameof(StatusToggleCommand), typeof(ICommand), typeof(TaskCard));

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public ICommand StatusToggleCommand
    {
        get => (ICommand)GetValue(StatusToggleCommandProperty);
        set => SetValue(StatusToggleCommandProperty, value);
    }

    public TaskCard()
    {
        InitializeComponent();
    }
}
