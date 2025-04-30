using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace eCommerce.OrdersMicroservice.BusinessLogicLayer.Policies;
public class PollyPolicies : IPollyPolicies
{
    private readonly ILogger<UsersMicroServicePolicies> _logger;

    public PollyPolicies(ILogger<UsersMicroServicePolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
 .WaitAndRetryAsync(
    retryCount: retryCount, //Number of retries
    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Delay between retries
    onRetry: (outcome, timespan, retryAttempt, context) =>
    {
        _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
    });

        return policy;
    }
    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak )
    {
        Polly.CircuitBreaker.AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
 .CircuitBreakerAsync(
    handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking, //Number of retries
    durationOfBreak: durationOfBreak, // Delay between retries
    onBreak: (outcome, timespan) =>
    {
        _logger.LogInformation($"Circuit Breaker opened for {timespan.TotalMinutes} minutes due to consecutive 3 failures. The subsequent requests will be blocked");
    }, 
    onReset:() =>
    {
        _logger.LogInformation($"Circuit Breaker closed. The subsequent requests will be allowed.");
    });

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);

        return policy;
    }
    
}

