namespace StarWars.Infrastructure.HttpClients;

using Polly;
using System.Net.Http;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} - Waiting {timespan.TotalSeconds}s");
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(10);
    }
}
