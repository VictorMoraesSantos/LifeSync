using System.Text.Json.Serialization;

namespace LifeSyncApp.Client.Models.Common
{
    public class HttpResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errors")]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("pagination")]
        public PaginationData? Pagination { get; set; }
    }

    public class HttpResult<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errors")]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("pagination")]
        public PaginationData? Pagination { get; set; }
    }

    public class PaginationData
    {
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
