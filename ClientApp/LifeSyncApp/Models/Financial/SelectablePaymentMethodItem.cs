using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.Models.Financial
{
    public partial class SelectablePaymentMethodItem : ObservableObject
    {
        public PaymentMethod Value { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string IconSource { get; set; } = "wallet.svg";

        [ObservableProperty]
        private bool _isSelected;
    }
}
