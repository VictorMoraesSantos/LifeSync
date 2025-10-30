namespace BuildingBlocks.Results
{
    public class PaginationData
    {
        public int? CurrentPage { get; private set; } = 1;
        public int? PageSize { get; private set; } = 50;
        public int? TotalItems { get; private set; } = 0;
        public int? TotalPages { get; private set; } = 0;

        public PaginationData(int? currentPage, int? pageSize, int? totalItems = null, int? totalPages = null)
        {
            CurrentPage = currentPage ?? 1;
            PageSize = pageSize ?? 50;
            TotalItems = totalItems ?? 0;
            TotalPages = totalPages ?? 0;
        }
    }
}
