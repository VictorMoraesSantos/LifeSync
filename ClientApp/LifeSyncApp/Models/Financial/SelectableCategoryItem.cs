using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.Models.Financial
{
    public partial class SelectableCategoryItem : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [ObservableProperty]
        private bool _isSelected;
    }
}
