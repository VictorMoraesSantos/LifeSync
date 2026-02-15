using System.ComponentModel;

namespace LifeSyncApp.Models.Financial
{
    public class SelectablePaymentMethodItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public PaymentMethod Value { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string IconSource { get; set; } = "wallet.svg";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
