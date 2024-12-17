namespace StarWars.Infrastructure.HttpClients;

using Polly;
using System.Threading.Tasks;

public class PollyHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy = PollyPolicies.RetryPolicy().WrapAsync(PollyPolicies.TimeoutPolicy());

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _policy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}