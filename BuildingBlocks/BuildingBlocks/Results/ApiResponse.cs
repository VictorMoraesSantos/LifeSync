namespace BuildingBlocks.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public int StatusCode { get; private set; }
        public string[] Errors { get; private set; } = Array.Empty<string>();
        public T Data { get; private set; }

        private ApiResponse() { }

        public static ApiResponse<T> FromResult(Result<T> result, int successCode = 200)
        {
            if (result.IsSuccess)
            {
                return new ApiResponse<T>
                {
                    Success = true,
                    StatusCode = successCode,
                    Data = result.Value
                };
            }

            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = DetermineStatusCode(result.Error),
                Errors = new[] { result.Error }
            };
        }

        private static int DetermineStatusCode(string error)
        {
            if (string.IsNullOrEmpty(error))
                return 400;

            var errorLower = error.ToLowerInvariant();

            if (errorLower.Contains("não encontrado") || errorLower.Contains("not found"))
                return 404;
            if (errorLower.Contains("não autorizado") || errorLower.Contains("unauthorized"))
                return 401;
            if (errorLower.Contains("proibido") || errorLower.Contains("forbidden"))
                return 403;
            if (errorLower.Contains("conflito") || errorLower.Contains("conflict"))
                return 409;

            return 400;
        }
    }
}