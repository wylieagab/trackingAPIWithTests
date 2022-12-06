namespace trackingAPI.MiddleWare
{
    public partial class RateLimitingMiddleware
    {
        public class ClientStatistics
        {
            public DateTime LastSuccessfulResponseTime { get; set; }
            public int NumberOfRequestsCompletedSuccessfully { get; set; }
        }
    }
}
