using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class ModalButtonBar : ContentView
{
    public static readonly BindableProperty CancelCommandProperty =
        BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(ModalButtonBar), null,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (ModalButtonBar)b;
                ctrl.CancelBorder.GestureRecognizers.Clear();
                if (n is ICommand cmd)
                    ctrl.CancelBorder.GestureRecognizers.Add(new TapGestureRecognizer { Command = cmd });
            });

    public static readonly BindableProperty SaveCommandProperty =
        BindableProperty.Create(nameof(SaveCommand), typeof(ICommand), typeof(ModalButtonBar), null,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (ModalButtonBar)b;
                ctrl.SaveBorder.GestureRecognizers.Clear();
                if (n is ICommand cmd)
                    ctrl.SaveBorder.GestureRecognizers.Add(new TapGestureRecognizer { Command = cmd });
            });

    public static readonly BindableProperty CancelTextProperty =
        BindableProperty.Create(nameof(CancelText), typeof(string), typeof(ModalButtonBar), "Cancelar",
            propertyChanged: (b, _, n) => ((ModalButtonBar)b).CancelLabel.Text = (string)n);

    public static readonly BindableProperty SaveTextProperty =
        BindableProperty.Create(nameof(SaveText), typeof(string), typeof(ModalButtonBar), "Salvar",
            propertyChanged: (b, _, n) => ((ModalButtonBar)b).SaveLabel.Text = (string)n);

    public ICommand CancelCommand { get => (ICommand)GetValue(CancelCommandProperty); set => SetValue(CancelCommandProperty, value); }
    public ICommand SaveCommand { get => (ICommand)GetValue(SaveCommandProperty); set => SetValue(SaveCommandProperty, value); }
    public string CancelText { get => (string)GetValue(CancelTextProperty); set => SetValue(CancelTextProperty, value); }
    public string SaveText { get => (string)GetValue(SaveTextProperty); set => SetValue(SaveTextProperty, value); }

    public ModalButtonBar()
    {
        InitializeComponent();
    }
}
