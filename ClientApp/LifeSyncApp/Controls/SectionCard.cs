namespace LifeSyncApp.Controls;

[ContentProperty(nameof(CardContent))]
public partial class SectionCard : ContentView
{
    public static readonly BindableProperty CardContentProperty =
        BindableProperty.Create(nameof(CardContent), typeof(View), typeof(SectionCard), null,
            propertyChanged: (b, _, n) => { var c = (SectionCard)b; if (c._initialized) c.CardBorder.Content = (View)n; });

    public static readonly BindableProperty CardPaddingProperty =
        BindableProperty.Create(nameof(CardPadding), typeof(Thickness), typeof(SectionCard), new Thickness(0),
            propertyChanged: (b, _, n) => { var c = (SectionCard)b; if (c._initialized) c.CardBorder.Padding = (Thickness)n; });

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(SectionCard), 16,
            propertyChanged: (b, _, n) =>
            {
                var c = (SectionCard)b;
                if (!c._initialized) return;
                var r = (int)n;
                c.CardBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(r) };
            });

    private bool _initialized;

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (CardBorder?.Content != null)
            CardBorder.Content.BindingContext = BindingContext;
    }

    public View CardContent { get => (View)GetValue(CardContentProperty); set => SetValue(CardContentProperty, value); }
    public Thickness CardPadding { get => (Thickness)GetValue(CardPaddingProperty); set => SetValue(CardPaddingProperty, value); }
    public int CornerRadius { get => (int)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

    public SectionCard()
    {
        InitializeComponent();
        _initialized = true;

        // Apply properties set before InitializeComponent
        if (CardContent != null) CardBorder.Content = CardContent;
        if (CardPadding != new Thickness(0)) CardBorder.Padding = CardPadding;
        if (CornerRadius != 16)
            CardBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(CornerRadius) };
    }
}
