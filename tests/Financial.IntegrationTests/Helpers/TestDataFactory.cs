using Bogus;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.IntegrationTests.Helpers
{
    public static class TestDataFactory
    {
        public static Category CreateCategory(int userId = 1)
        {
            var faker = new Faker<Category>()
                .CustomInstantiator(f => new Category(
                    userId,
                    f.Commerce.Categories(1)[0],
                    f.Lorem.Sentence()));

            return faker.Generate();
        }

        public static Transaction CreateTransaction(int userId = 1, int? categoryId = null, bool isRecurring = false)
        {
            var faker = new Faker<Transaction>()
                .CustomInstantiator(f => new Transaction(
                    userId,
                    categoryId,
                    f.PickRandom<PaymentMethod>(),
                    f.PickRandom<TransactionType>(),
                    Money.Create(f.Random.Int(100, 10000), Currency.BRL),
                    f.Commerce.ProductName(),
                    DateTime.UtcNow,
                    isRecurring));

            return faker.Generate();
        }

        public static RecurrenceSchedule CreateRecurrenceSchedule(
            int transactionId = 1,
            RecurrenceFrequency frequency = RecurrenceFrequency.Monthly,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxOccurrences = null)
        {
            return new RecurrenceSchedule(
                transactionId,
                frequency,
                startDate ?? DateTime.UtcNow,
                endDate,
                maxOccurrences);
        }

        public static List<RecurrenceSchedule> CreateRecurrenceSchedules(int count, int startTransactionId = 1)
        {
            var schedules = new List<RecurrenceSchedule>();
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                schedules.Add(new RecurrenceSchedule(
                    startTransactionId + i,
                    faker.PickRandom<RecurrenceFrequency>(),
                    DateTime.UtcNow));
            }

            return schedules;
        }
    }
}
