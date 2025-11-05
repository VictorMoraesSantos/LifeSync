namespace LifeSyncApp.Client.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public T? Data { get; set; }
    public PaginationData? Pagination { get; set; }
}

public class PaginationData
{
    public int? CurrentPage { get; set; }
    public int? PageSize { get; set; }
    public int? TotalItems { get; set; }
    public int? TotalPages { get; set; }
}
