using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class CustomTabBar : ContentView, INotifyPropertyChanged
{
    private int _selectedTab;

    public int SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab != value)
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SelectTabCommand { get; }

    public event EventHandler<int> TabSelected;

    public CustomTabBar()
    {
        InitializeComponent();
        SelectTabCommand = new Command<string>(OnTabSelected);
        BindingContext = this;
        SelectedTab = 0;
    }

    private void OnTabSelected(string tabIndexStr)
    {
        if (int.TryParse(tabIndexStr, out int tabIndex))
        {
            SelectedTab = tabIndex;
            TabSelected?.Invoke(this, tabIndex);
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;

    protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
