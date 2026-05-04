using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public enum DateFilterOption
    {
        All, Today, ThisWeek, ThisMonth
    }

    public class FilterAppliedEventArgs : EventArgs
    {
        public Status? Status { get; }
        public Priority? Priority { get; }
        public DateFilterOption? DateFilter { get; }

        public FilterAppliedEventArgs(Status? status, Priority? priority, DateFilterOption? dateFilter)
        {
            Status = status;
            Priority = priority;
            DateFilter = dateFilter;
        }
    }

    public partial class FilterTaskItemViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _selectedStatus = "";

        [ObservableProperty]
        private string _selectedPriority = "";

        [ObservableProperty]
        private string _selectedDateFilter = "";

        public event EventHandler<FilterAppliedEventArgs>? FiltersApplied;
        public event EventHandler? Closed;

        public FilterTaskItemViewModel()
        {
        }

        [RelayCommand]
        private void CloseModal() => Closed?.Invoke(this, EventArgs.Empty);

        [RelayCommand]
        private void SelectStatus(string status) => SelectedStatus = status ?? "";

        [RelayCommand]
        private void SelectPriority(string priority) => SelectedPriority = priority ?? "";

        [RelayCommand]
        private void SelectDateFilter(string dateOption) => SelectedDateFilter = dateOption ?? "";

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedStatus = "";
            SelectedPriority = "";
            SelectedDateFilter = "";
        }

        [RelayCommand]
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

            FiltersApplied?.Invoke(this, new FilterAppliedEventArgs(statusEnum, priorityEnum, dateFilterEnum));
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
