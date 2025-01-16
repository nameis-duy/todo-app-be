namespace Application.DTOs.Base
{
#pragma warning disable CS8618
    public class ResponseResult<T>
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; } = default;
        public Exception? Exception { get; set; } = null!;
    }
}
