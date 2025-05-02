namespace LQM.Web.Api.Models
{
    public class FileContentRes
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string Extension { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
