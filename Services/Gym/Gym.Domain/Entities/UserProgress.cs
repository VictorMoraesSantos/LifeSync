using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.Events;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class UserProgress : BaseEntity<int>, IAggregateRoot
    {
        public int UserId { get; private set; }
        public DateTime RecordDate { get; private set; }

        private readonly List<ProgressMetric> _metrics;
        public IReadOnlyCollection<ProgressMetric> Metrics => _metrics.AsReadOnly();

        private readonly List<ProgressNote> _notes;
        public IReadOnlyCollection<ProgressNote> Notes => _notes.AsReadOnly();

        protected UserProgress()
        {
            _metrics = new List<ProgressMetric>();
            _notes = new List<ProgressNote>();
        }

        public UserProgress(int userId, DateTime recordDate)
        {
            if (userId <= 0)
                throw new DomainException("User ID cannot be negative");

            if (recordDate > DateTime.UtcNow)
                throw new DomainException("Record date cannot be in the future");

            UserId = userId;
            RecordDate = recordDate;

            _metrics = new List<ProgressMetric>();
            _notes = new List<ProgressNote>();

            AddDomainEvent(new ProgressRecordedEvent(Id, userId));
        }

        // Métodos de negócio
        public void AddMetric(string name, decimal value, MeasurementUnit unit, string notes = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Metric name cannot be empty");

            var existingMetric = _metrics.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingMetric != null)
                throw new DomainException($"Metric '{name}' already exists");

            var metric = new ProgressMetric(name, value, unit, notes);
            _metrics.Add(metric);
        }

        public void UpdateMetric(string name, decimal value, MeasurementUnit unit, string notes = null)
        {
            var metric = _metrics.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (metric == null)
                throw new DomainException($"Metric '{name}' not found");

            var updatedMetric = new ProgressMetric(name, value, unit, notes);

            _metrics.Remove(metric);
            _metrics.Add(updatedMetric);
        }

        public void RemoveMetric(string name)
        {
            var metric = _metrics.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (metric != null)
                _metrics.Remove(metric);
        }

        public void AddNote(string content, string category = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Note content cannot be empty");

            var note = new ProgressNote(content, category);
            _notes.Add(note);
        }

        public void UpdateNote(int noteId, string content, string category = null)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            if (note == null)
                throw new DomainException("Note not found");

            note.Update(content, category);
        }

        public void RemoveNote(int noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
                _notes.Remove(note);
        }
    }
}
