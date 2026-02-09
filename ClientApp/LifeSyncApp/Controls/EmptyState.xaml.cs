using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class EmptyState : ContentView
{
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(EmptyState),
            "empty_box.svg");

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(EmptyState),
            "Nenhum dado encontrado");

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(EmptyState),
            "Não há informações para exibir no momento.");

    public static readonly BindableProperty ShowActionButtonProperty =
        BindableProperty.Create(
            nameof(ShowActionButton),
            typeof(bool),
            typeof(EmptyState),
            false);

    public static readonly BindableProperty ActionButtonTextProperty =
        BindableProperty.Create(
            nameof(ActionButtonText),
            typeof(string),
            typeof(EmptyState),
            "Adicionar");

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(
            nameof(ActionCommand),
            typeof(ICommand),
            typeof(EmptyState),
            null);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool ShowActionButton
    {
        get => (bool)GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }

    public string ActionButtonText
    {
        get => (string)GetValue(ActionButtonTextProperty);
        set => SetValue(ActionButtonTextProperty, value);
    }

    public ICommand ActionCommand
    {
        get => (ICommand)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public EmptyState()
    {
        InitializeComponent();
    }
}
