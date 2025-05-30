﻿using System.Net;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Results
{
    public class HttpResult : HttpResult<object>
    {
        public HttpResult() : base() { }

        public HttpResult(object data) : base(data) { }

        public HttpResult(HttpStatusCode statusCode, params string[] errors)
            : base(statusCode, errors) { }

        public static HttpResult Ok(object data) => new(data) { StatusCode = HttpStatusCode.OK };
        public static HttpResult Ok() => new() { StatusCode = HttpStatusCode.OK };

        public static HttpResult Created(object data) => new(data) { StatusCode = HttpStatusCode.Created };
        public static HttpResult Created() => new() { StatusCode = HttpStatusCode.Created };

        public static HttpResult Updated() => new() { StatusCode = HttpStatusCode.NoContent };
        public static HttpResult Deleted() => new() { StatusCode = HttpStatusCode.NoContent };

        public static HttpResult BadRequest(params string[] errors) => new(HttpStatusCode.BadRequest, errors);
        public static HttpResult NotFound(params string[] errors) => new(HttpStatusCode.NotFound, errors);
        public static HttpResult Forbidden(params string[] errors) => new(HttpStatusCode.Forbidden, errors);
        public static HttpResult InternalError(params string[] errors) => new(HttpStatusCode.InternalServerError, errors);
    }

    public class HttpResult<T>
    {
        public HttpResult()
        {
            Errors = Array.Empty<string>();
        }

        public HttpResult(T data)
        {
            Data = data;
            Errors = Array.Empty<string>();
            StatusCode = HttpStatusCode.OK;
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

        public void AddError(params string[] errors)
        {
            if (errors == null || errors.Length == 0) return;
            Errors = Errors.Concat(errors.Where(e => !string.IsNullOrWhiteSpace(e))).ToArray();
        }

        public static HttpResult<T> Ok(T data) => new(data) { StatusCode = HttpStatusCode.OK };
        public static HttpResult<T> Created(T data) => new(data) { StatusCode = HttpStatusCode.Created };

        public static HttpResult<T> Updated() => new() { StatusCode = HttpStatusCode.NoContent };
        public static HttpResult<T> Deleted() => new() { StatusCode = HttpStatusCode.NoContent };

        public static HttpResult<T> BadRequest(params string[] errors) => new(HttpStatusCode.BadRequest, errors);
        public static HttpResult<T> NotFound(params string[] errors) => new(HttpStatusCode.NotFound, errors);
        public static HttpResult<T> Forbidden(params string[] errors) => new(HttpStatusCode.Forbidden, errors);
        public static HttpResult<T> InternalError(params string[] errors) => new(HttpStatusCode.InternalServerError, errors);

        public static HttpResult<T> FromException(Exception ex)
        {
            var errors = new[]
            {
                ex.Message,
                ex.InnerException?.Message
            }.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();

            return new HttpResult<T>(HttpStatusCode.InternalServerError, errors);
        }
    }
}