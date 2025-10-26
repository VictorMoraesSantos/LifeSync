namespace Core.Domain.Filters
{
    public interface IDomainQueryFilter<T>
    {
        public T? Id { get; }
        public DateTime? CreatedAt { get; }
        public DateTime? UpdatedAt { get; }
        public bool? IsDeleted { get; }
    }
}
