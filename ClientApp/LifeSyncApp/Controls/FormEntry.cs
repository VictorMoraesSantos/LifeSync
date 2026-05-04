namespace LifeSyncApp.Controls;

public partial class FormEntry : ContentView
{
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(FormEntry), string.Empty,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldLabel.Text = (string)n; });

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FormEntry), string.Empty,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldEntry.Placeholder = (string)n; });

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FormEntry), string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (FormEntry)b;
                if (!ctrl._initialized || ctrl._isUpdating) return;
                ctrl._isUpdating = true;
                ctrl.FieldEntry.Text = (string)(n ?? string.Empty);
                ctrl._isUpdating = false;
            });

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FormEntry), false,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldEntry.IsPassword = (bool)n; });

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FormEntry), Keyboard.Default,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldEntry.Keyboard = (Keyboard)n; });

    public static readonly BindableProperty ReturnTypeProperty =
        BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(FormEntry), ReturnType.Default,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldEntry.ReturnType = (ReturnType)n; });

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(FormEntry), int.MaxValue,
            propertyChanged: (b, _, n) => { var c = (FormEntry)b; if (c._initialized) c.FieldEntry.MaxLength = (int)n; });

    public string LabelText { get => (string)GetValue(LabelTextProperty); set => SetValue(LabelTextProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }
    public ReturnType ReturnType { get => (ReturnType)GetValue(ReturnTypeProperty); set => SetValue(ReturnTypeProperty, value); }
    public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

    public Entry InternalEntry => FieldEntry;

    private bool _isUpdating;
    private bool _initialized;

    public FormEntry()
    {
        InitializeComponent();
        _initialized = true;

        // Apply any properties set before InitializeComponent
        FieldLabel.Text = LabelText;
        FieldEntry.Placeholder = Placeholder;
        FieldEntry.Text = Text ?? string.Empty;
        FieldEntry.IsPassword = IsPassword;
        FieldEntry.Keyboard = Keyboard;
        FieldEntry.ReturnType = ReturnType;
        if (MaxLength != int.MaxValue) FieldEntry.MaxLength = MaxLength;

        FieldEntry.TextChanged += (s, e) =>
        {
            if (_isUpdating) return;
            _isUpdating = true;
            Text = e.NewTextValue;
            _isUpdating = false;
        };
    }
}
