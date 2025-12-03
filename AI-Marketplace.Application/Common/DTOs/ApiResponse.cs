namespace AI_Marketplace.Application.Common.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public IEnumerable<string>? Errors { get; init; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new ApiResponse<T> { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(IEnumerable<string> errors, string? message = null) =>
            new ApiResponse<T> { Success = false, Errors = errors, Message = message };
    }
}