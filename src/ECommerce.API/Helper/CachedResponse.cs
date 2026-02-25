namespace ECommerce.API.Helper
{
    public class CachedResponse
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = default!;
        public string Body { get; set; } = default!;
    }
}
