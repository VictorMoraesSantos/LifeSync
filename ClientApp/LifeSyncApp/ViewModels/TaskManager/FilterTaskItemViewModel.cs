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

        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus == value) return;
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        private string _selectedPriority;
        public string SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                if (_selectedPriority == value) return;
                _selectedPriority = value;
                OnPropertyChanged(nameof(SelectedPriority));
            }
        }

        private string _selectedDateFilter;
        public string SelectedDateFilter
        {
            get => _selectedDateFilter;
            set
            {
                if (_selectedDateFilter == value) return;
                _selectedDateFilter = value;
                OnPropertyChanged(nameof(SelectedDateFilter));
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

            SelectedStatus = "";
            SelectedPriority = "";
            SelectedDateFilter = "";

            CloseModalCommand = new Command(() => _onCloseModal?.Invoke());
            SelectStatusCommand = new Command<string>(SelectStatus);
            SelectPriorityCommand = new Command<string>(SelectPriority);
            SelectDateFilterCommand = new Command<string>(SelectDateFilter);
            ClearFiltersCommand = new Command(ClearFilters);
            ApplyFiltersCommand = new Command(ApplyFilters);
        }

        private void SelectStatus(string status)
        {
            SelectedStatus = status ?? "";
        }

        private void SelectPriority(string priority)
        {
            SelectedPriority = priority ?? "";
        }

        private void SelectDateFilter(string dateOption)
        {
            SelectedDateFilter = dateOption ?? "";
        }

        private void ClearFilters()
        {
            SelectedStatus = "";
            SelectedPriority = "";
            SelectedDateFilter = "";
        }

        private void ApplyFilters()
        {
            Status? statusEnum = SelectedStatus switch
            {
                "Pending" => Status.Pending,
                "InProgress" => Status.InProgress,
                "Completed" => Status.Completed,
                _ => null
            };

            Priority? priorityEnum = SelectedPriority switch
            {
                "Low" => Priority.Low,
                "Medium" => Priority.Medium,
                "High" => Priority.High,
                "Urgent" => Priority.Urgent,
                _ => null
            };

            DateFilterOption? dateFilterEnum = SelectedDateFilter switch
            {
                "Today" => DateFilterOption.Today,
                "ThisWeek" => DateFilterOption.ThisWeek,
                "ThisMonth" => DateFilterOption.ThisMonth,
                "" => DateFilterOption.All,
                _ => DateFilterOption.All
            };

            _onApplyFilters?.Invoke(statusEnum, priorityEnum, dateFilterEnum);
            _onCloseModal?.Invoke();
        }
    }
}
