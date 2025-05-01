namespace Shared.Space.Lib.Models
{
    public class Response<T>
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "Success";
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
