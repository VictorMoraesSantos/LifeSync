using System.Net;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Results
{
    public class HttpResult : HttpResult<object>
    {
        public HttpResult() : base() { }

        public HttpResult(HttpStatusCode statusCode) : base(statusCode) { }
        public HttpResult(object data, HttpStatusCode statusCode) : base(data, statusCode) { }
        public HttpResult(HttpStatusCode statusCode, params string[] errors) : base(statusCode, errors) { }

        public static HttpResult Ok(object data) => new HttpResult(data, HttpStatusCode.OK);
        public static HttpResult Ok(HttpStatusCode statusCode) => new HttpResult(HttpStatusCode.OK);
        public static HttpResult Created(object data) => new HttpResult(data, HttpStatusCode.Created);
        public static HttpResult Created(HttpStatusCode statusCode) => new HttpResult(HttpStatusCode.Created);
        public static HttpResult Updated(HttpStatusCode statusCode) => new HttpResult(HttpStatusCode.NoContent);
        public static HttpResult Deleted(HttpStatusCode statusCode) => new HttpResult(HttpStatusCode.NoContent);
        public static HttpResult BadRequest(params string[] errors) => new HttpResult(HttpStatusCode.BadRequest, errors);
        public static HttpResult NotFound(params string[] errors) => new HttpResult(HttpStatusCode.NotFound, errors);
        public static HttpResult Forbidden(params string[] errors) => new HttpResult(HttpStatusCode.Forbidden, errors);
        public static HttpResult InternalError(params string[] errors) => new HttpResult(HttpStatusCode.InternalServerError, errors);
    }

    public class HttpResult<T>
    {
        public HttpResult()
        {
            Errors = Array.Empty<string>();
        }

        public HttpResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpResult(T data, HttpStatusCode statusCode)
        {
            Data = data;
            Errors = Array.Empty<string>();
            StatusCode = statusCode;
        }

        public HttpResult(T data, PaginationData pagination, HttpStatusCode statusCode)
            : this(data, statusCode)
        {
            Pagination = pagination;
        }

        public HttpResult(HttpStatusCode statusCode, params string[] errors)
        {
            StatusCode = statusCode;
            Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() ?? Array.Empty<string>();
        }

        [JsonPropertyOrder(0)]
        public bool Success => ((int)StatusCode >= 200 && (int)StatusCode < 300) && Errors.Length == 0;

        [JsonPropertyOrder(1)]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        [JsonPropertyOrder(2)]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [JsonPropertyOrder(3)]
        public T Data { get; set; }

        [JsonPropertyOrder(4)]
        public PaginationData? Pagination { get; set; }

        public void AddError(params string[] errors)
        {
            if (errors == null || errors.Length == 0) return;
            Errors = Errors.Concat(errors.Where(e => !string.IsNullOrWhiteSpace(e))).ToArray();
        }

        public static HttpResult<T> Ok(T data) => new HttpResult<T>(data, HttpStatusCode.OK);
        public static HttpResult<T> Created(T data) => new HttpResult<T>(data, HttpStatusCode.Created);
        public static HttpResult<T> Updated() => new HttpResult<T>(HttpStatusCode.NoContent);
        public static HttpResult<T> Deleted() => new HttpResult<T>(HttpStatusCode.NoContent);
        public static HttpResult<T> BadRequest(params string[] errors) => new HttpResult<T>(HttpStatusCode.BadRequest, errors);
        public static HttpResult<T> NotFound(params string[] errors) => new HttpResult<T>(HttpStatusCode.NotFound, errors);
        public static HttpResult<T> Forbidden(params string[] errors) => new HttpResult<T>(HttpStatusCode.Forbidden, errors);
        public static HttpResult<T> InternalError(params string[] errors) => new HttpResult<T>(HttpStatusCode.InternalServerError, errors);

        public static HttpResult<T> FromException(Exception ex)
        {
            var errors = new[]
            {
                ex.Message,
                ex.InnerException?.Message
            }.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();

            return new HttpResult<T>(HttpStatusCode.InternalServerError, errors);
        }

        public static HttpResult<T> Ok(T data, PaginationData pagination)
            => new HttpResult<T>(data, pagination, HttpStatusCode.OK);

        public static HttpResult<T> Ok(T data, int currentPage, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            return new HttpResult<T>(
                data,
                new PaginationData(currentPage, pageSize, totalItems, totalPages),
                HttpStatusCode.OK);
        }

        public static HttpResult<T> FromResult<TData>(Result<(TData Items, PaginationData Pagination)> result)
            where TData : T
        {
            if (!result.IsSuccess)
                return BadRequest(result.Error!.Description);

            return Ok(result.Value.Items, result.Value.Pagination);
        }
    }

    public class PaginationData
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }

        public PaginationData(int currentPage = 1, int pageSize = 50, int totalItems = 0, int totalPages = 0)
        {
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;
        }
    }
}