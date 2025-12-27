using BuildingBlocks.Results;

namespace Core.Domain.Notifications
{
    public abstract class Notifiable
    {
        private readonly Notification _notification = new();
        public IReadOnlyCollection<Error> Errors => _notification.Errors;
        public bool IsValid => _notification.IsValid;
        public bool IsInvalid => !IsValid;
        protected void AddError(Error error) => _notification.AddError(error);
        protected void AddErrors(IEnumerable<Error> errors) => _notification.AddErrors(errors);
        protected void ClearErrors() => _notification.Clear();
    }
}
