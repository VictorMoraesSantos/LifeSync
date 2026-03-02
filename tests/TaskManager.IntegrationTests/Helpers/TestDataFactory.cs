using Bogus;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.IntegrationTests.Helpers
{
    public static class TestDataFactory
    {
        private static readonly Faker<TaskItem> _taskItemFaker = new Faker<TaskItem>()
            .CustomInstantiator(f => new TaskItem(
                f.Lorem.Sentence(3, 5),
                f.Lorem.Paragraph(),
                f.PickRandom<Priority>(),
                DateOnly.FromDateTime(f.Date.Future()),
                f.Random.Int(1, 100),
                null
            ));

        private static readonly Faker<TaskLabel> _taskLabelFaker = new Faker<TaskLabel>()
            .CustomInstantiator(f => new TaskLabel(
                f.Commerce.Categories(1).First(),
                f.PickRandom<LabelColor>(),
                f.Random.Int(1, 100)
            ));

        public static TaskItem CreateTaskItem(int? userId = null)
        {
            var item = _taskItemFaker.Generate();
            if (userId.HasValue)
            {
                return new TaskItem(
                    item.Title,
                    item.Description,
                    item.Priority,
                    item.DueDate,
                    userId.Value,
                    null);
            }
            return item;
        }

        public static List<TaskItem> CreateTaskItems(int count, int? userId = null)
        {
            if (userId.HasValue)
            {
                return Enumerable.Range(1, count)
                    .Select(_ => CreateTaskItem(userId))
                    .ToList();
            }
            return _taskItemFaker.Generate(count);
        }

        public static TaskLabel CreateTaskLabel(int? userId = null)
        {
            var label = _taskLabelFaker.Generate();
            if (userId.HasValue)
            {
                return new TaskLabel(label.Name, label.LabelColor, userId.Value);
            }
            return label;
        }

        public static List<TaskLabel> CreateTaskLabels(int count, int? userId = null)
        {
            if (userId.HasValue)
            {
                return Enumerable.Range(1, count)
                    .Select(_ => CreateTaskLabel(userId))
                    .ToList();
            }
            return _taskLabelFaker.Generate(count);
        }
    }
}
