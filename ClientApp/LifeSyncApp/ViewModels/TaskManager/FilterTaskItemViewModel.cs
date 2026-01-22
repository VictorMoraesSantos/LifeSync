using LifeSyncApp.Models.TaskManager.Enums;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class FilterTaskItemViewModel : BaseViewModel
    {
        public enum DateFilterOption
        {
            All, Today, ThisWeek, ThisMonth
        }

        private Status? _selectedStatus { get; set; }
        public Status? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        private Priority? _selectedPriority { get; set; }
        public Priority? SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                _selectedPriority = value;
                OnPropertyChanged(nameof(SelectedPriority));
            }
        }

        private DateFilterOption? _selectedDateOption { get; set; }
        public DateFilterOption? SelectedDateOption
        {
            get => _selectedDateOption;
            set
            {
                _selectedDateOption = value;
                OnPropertyChanged(nameof(SelectedDateOption));
            }
        }

        public ICommand CloseModalCommand { get; set; }
        public ICommand SelectStatusCommand { get; set; }
        public ICommand SelectPriorityCommand { get; set; }
        public ICommand SelectDateFilterCommand { get; set; }
        public ICommand ClearFiltersCommand { get; set; }
        public ICommand ApplyFiltersCommand { get; set; }

        private Action<Status?, Priority?, DateFilterOption?> _onApplyFilters;
        private Action _onCloseModal;

        public FilterTaskItemViewModel(Action<Status?, Priority?, DateFilterOption?> onApplyFilters, Action onCloseModal)
        {
            _onApplyFilters = onApplyFilters;
            _onCloseModal = onCloseModal;

            CloseModalCommand = new Command(() => _onCloseModal?.Invoke());
            SelectStatusCommand = new Command<string>(SelectStatus);
            SelectPriorityCommand = new Command<string>(SelectPriority);
            SelectDateFilterCommand = new Command<string>(SelectDateFilter);
            ClearFiltersCommand = new Command(ClearFilters);
            ApplyFiltersCommand = new Command(ApplyFilters);
        }

        private void SelectStatus(string status)
        {
            SelectedStatus = status switch
            {
                "Pending" => Status.Pending,
                "InProgress" => Status.InProgress,
                "Completed" => Status.Completed,
                _ => null
            };
        }

        private void SelectPriority(string priority)
        {
            SelectedPriority = priority switch
            {
                "Low" => Priority.Low,
                "Medium" => Priority.Medium,
                "High" => Priority.High,
                "Urgent" => Priority.Urgent,
                _ => null
            };
        }

        private void SelectDateFilter(string dateOption)
        {
            SelectedDateOption = dateOption switch
            {
                "All" => DateFilterOption.All,
                "Today" => DateFilterOption.Today,
                "ThisWeek" => DateFilterOption.ThisWeek,
                "ThisMonth" => DateFilterOption.ThisMonth,
                _ => null
            };
        }

        private void ClearFilters()
        {
            SelectedStatus = null;
            SelectedPriority = null;
            SelectedDateOption = DateFilterOption.All;
        }

        private void ApplyFilters()
        {
            _onApplyFilters?.Invoke(SelectedStatus, SelectedPriority, SelectedDateOption);
            _onCloseModal?.Invoke();
        }
    }
}
