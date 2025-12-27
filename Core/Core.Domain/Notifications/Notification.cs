using BuildingBlocks.Results;

namespace Core.Domain.Notifications
{
    public class Notification
    {
        private readonly List<Error> _errors = new();
        public IReadOnlyCollection<Error> Errors => _errors.AsReadOnly();
        public bool HasErrors => _errors.Any();
        public bool IsValid => !HasErrors;

        public void AddError(Error error)
        {
            if (error != null)
                _errors.Add(error);
        }

        public void AddErrors(IEnumerable<Error> errors)
        {
            if (errors != null)
                _errors.AddRange(errors);
        }

        public void Clear() => _errors.Clear();
    }
}
