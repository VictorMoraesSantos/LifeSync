using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.Models.Financial;

public partial class Category : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private string color = "#6366F1";

    [ObservableProperty]
    private string icon = "📋";

    [ObservableProperty]
    private int userId;

    [ObservableProperty]
    private DateTime createdAt;

    [ObservableProperty]
    private DateTime? updatedAt;
}
