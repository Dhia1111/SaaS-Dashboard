namespace APIs.Responses

{
    public class ApiResult<T>
    {
        public bool Success { get; init; }
        public string Code { get; init; } = default!;
        public string Message { get; init; } = default!;
        public T? Data { get; init; }

        public static ApiResult<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Code = "OK", Message = message, Data = data };

        public static ApiResult<T> Fail(string code, string message) =>
            new() { Success = false, Code = code, Message = message };
    }
}
