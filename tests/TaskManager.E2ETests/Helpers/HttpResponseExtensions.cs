using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.E2ETests.Helpers
{
    public static class HttpResponseExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static async Task<T?> DeserializeDataAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var envelope = JsonSerializer.Deserialize<HttpResultEnvelope<T>>(content, JsonOptions);
            return envelope != null ? envelope.Data : default;
        }

        public static async Task<HttpResultEnvelope<T>?> DeserializeEnvelopeAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<HttpResultEnvelope<T>>(content, JsonOptions);
        }

        public static StringContent ToJsonContent<T>(this T obj)
        {
            var json = JsonSerializer.Serialize(obj, JsonOptions);
            return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }
    }

    public class HttpResultEnvelope<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string[] Errors { get; set; } = [];
        public T? Data { get; set; }
        public PaginationEnvelope? Pagination { get; set; }
    }

    public class PaginationEnvelope
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
