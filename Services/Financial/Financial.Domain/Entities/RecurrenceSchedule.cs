using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Domain.Entities
{
    public class RecurrenceSchedule : BaseEntity<int>
    {
        public int TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public RecurrenceFrequency Frequency { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime NextOccurrence { get; private set; }
        public int? MaxOccurrences { get; private set; }
        public int OccurrencesGenerated { get; private set; } = 0;
        public bool IsActive { get; private set; }

        protected RecurrenceSchedule() { }

        public RecurrenceSchedule(
            int transactionId,
            RecurrenceFrequency frequency,
            DateTime startDate,
            DateTime? endDate = null,
            int? maxOccurrences = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transactionId);

            if (endDate.HasValue && endDate.Value <= startDate)
                throw new DomainException(RecurrenceScheduleErrors.EndDateBeforeStartDate);

            if (maxOccurrences.HasValue && maxOccurrences.Value <= 0)
                throw new DomainException(RecurrenceScheduleErrors.InvalidMaxOccurrences);

            TransactionId = transactionId;
            Frequency = frequency;
            StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            EndDate = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : endDate;
            MaxOccurrences = maxOccurrences;
            NextOccurrence = CalculateNextOccurrence(StartDate, Frequency);
            IsActive = true;
        }

        public void Update(
            RecurrenceFrequency frequency,
            DateTime? endDate = null,
            int? maxOccurrences = null)
        {
            if (endDate.HasValue && endDate.Value <= StartDate)
                throw new DomainException(RecurrenceScheduleErrors.EndDateBeforeStartDate);

            if (maxOccurrences.HasValue && maxOccurrences.Value <= 0)
                throw new DomainException(RecurrenceScheduleErrors.InvalidMaxOccurrences);

            Frequency = frequency;
            EndDate = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : endDate;
            MaxOccurrences = maxOccurrences;

            MarkAsUpdated();
        }

        public Transaction GenerateTransaction()
        {
            if (!IsActive)
                throw new DomainException(RecurrenceScheduleErrors.InactiveSchedule);

            if (!CanGenerateNext())
                throw new DomainException(RecurrenceScheduleErrors.MaxOccurrencesReached);

            if (Transaction == null)
                throw new DomainException(RecurrenceScheduleErrors.TransactionNotLoaded);

            var transaction = new Transaction(
                Transaction.UserId,
                Transaction.CategoryId,
                Transaction.PaymentMethod,
                Transaction.TransactionType,
                Money.Create(Transaction.Amount.Amount, Transaction.Amount.Currency),
                Transaction.Description,
                NextOccurrence,
                isRecurring: false);

            OccurrencesGenerated++;
            NextOccurrence = CalculateNextOccurrence(NextOccurrence, Frequency);

            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value)
                IsActive = false;

            if (EndDate.HasValue && NextOccurrence > EndDate.Value)
                IsActive = false;

            MarkAsUpdated();

            return transaction;
        }

        public void SkipOccurrence()
        {
            if (!IsActive)
                throw new DomainException(RecurrenceScheduleErrors.InactiveSchedule);

            NextOccurrence = CalculateNextOccurrence(NextOccurrence, Frequency);

            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value)
                IsActive = false;

            if (EndDate.HasValue && NextOccurrence > EndDate.Value)
                IsActive = false;

            MarkAsUpdated();
        }

        public bool CanGenerateNext()
        {
            if (!IsActive) return false;
            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value) return false;
            if (EndDate.HasValue && NextOccurrence > EndDate.Value) return false;
            return true;
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value)
                throw new DomainException(RecurrenceScheduleErrors.CannotReactivate);

            if (EndDate.HasValue && NextOccurrence > EndDate.Value)
                throw new DomainException(RecurrenceScheduleErrors.CannotReactivate);

            IsActive = true;
            MarkAsUpdated();
        }

        private static DateTime CalculateNextOccurrence(DateTime current, RecurrenceFrequency frequency)
        {
            return frequency switch
            {
                RecurrenceFrequency.Daily => current.AddDays(1),
                RecurrenceFrequency.Weekly => current.AddDays(7),
                RecurrenceFrequency.Monthly => current.AddMonths(1),
                RecurrenceFrequency.Yearly => current.AddYears(1),
                _ => throw new DomainException(RecurrenceScheduleErrors.InvalidFrequency)
            };
        }
    }
}
