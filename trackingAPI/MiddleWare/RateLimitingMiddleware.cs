using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using trackingAPI.MiddleWare.Decorators;
using trackingAPI.MiddleWare.Extensions;

namespace trackingAPI.MiddleWare
{
    public partial class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public RateLimitingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            // read the LimitRequest attribute from the endpoint
            var rateLimitDecorator = endpoint?.Metadata.GetMetadata<LimitRequest>();
            if (rateLimitDecorator is null)
            {
                await _next(context);
                return;
            }

            var key = GenerateClientKey(context);
            var _clientStatistics = GetClientStatisticsByKey(key).Result;

            // Check whether the request violates the rate limit policy
            if (_clientStatistics != null
                && DateTime.UtcNow < _clientStatistics.LastSuccessfulResponseTime.AddSeconds(rateLimitDecorator.TimeWindow)
                && _clientStatistics.NumberOfRequestsCompletedSuccessfully == rateLimitDecorator.MaxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }
            await UpdateClientStatisticsAsync(key, rateLimitDecorator.MaxRequests);
            await _next(context);
        }

        private static string GenerateClientKey(HttpContext context)
            => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

        private async Task<ClientStatistics> GetClientStatisticsByKey(string key)
        {
            return await _cache.GetCachedValueAsync<ClientStatistics>(key);
        }

        private async Task UpdateClientStatisticsAsync(string key, int maxRequests)
        {
            var _clientStats = _cache.GetCachedValueAsync<ClientStatistics>(key).Result;
            if (_clientStats is not null)
            {
                _clientStats.LastSuccessfulResponseTime = DateTime.UtcNow;
                if (_clientStats.NumberOfRequestsCompletedSuccessfully == maxRequests)
                    _clientStats.NumberOfRequestsCompletedSuccessfully = 1;
                else
                    _clientStats.NumberOfRequestsCompletedSuccessfully++;

                await _cache.SetCachedValueAsync<ClientStatistics>(key, _clientStats);
            }
            else
            {
                var clientStats = new ClientStatistics
                {
                    LastSuccessfulResponseTime = DateTime.UtcNow,
                    NumberOfRequestsCompletedSuccessfully = 1
                };

                await _cache.SetCachedValueAsync<ClientStatistics>(key, clientStats);
            }
        }
    }
}
