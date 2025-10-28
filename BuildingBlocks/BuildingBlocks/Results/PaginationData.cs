namespace BuildingBlocks.Results
{
    public class PaginationData
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }

        public PaginationData(int currentPage = 1, int pageSize = 50, int totalItems = 0, int totalPages = 0)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
