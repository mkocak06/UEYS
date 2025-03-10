namespace Core.Models.LogInformation
{
    public class LogInformation
    {
        public long? UserId { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string StatusCode { get; set; }
        public int Count { get; set; }
    }
}