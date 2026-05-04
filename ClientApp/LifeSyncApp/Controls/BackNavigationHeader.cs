using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class BackNavigationHeader : ContentView
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BackNavigationHeader), null,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (BackNavigationHeader)b;
                ctrl.HeaderLayout.GestureRecognizers.Clear();
                if (n is ICommand cmd)
                    ctrl.HeaderLayout.GestureRecognizers.Add(new TapGestureRecognizer { Command = cmd });
            });

    public static readonly BindableProperty BackTextProperty =
        BindableProperty.Create(nameof(BackText), typeof(string), typeof(BackNavigationHeader), "Voltar",
            propertyChanged: (b, _, n) => ((BackNavigationHeader)b).BackLabel.Text = (string)n);

    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public string BackText { get => (string)GetValue(BackTextProperty); set => SetValue(BackTextProperty, value); }

    public BackNavigationHeader()
    {
        InitializeComponent();
    }
}
