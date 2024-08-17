using Microsoft.Extensions.Logging;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    private const int MAX_REQUEST_PER_MINUTE = 120;

    private uint _outboundWindow = MAX_REQUEST_PER_MINUTE;
    private long _windowResetsAtTicks;

    private async ValueTask WaitForOutboundRateLimitsAsync(CancellationToken token = default)
    {
        start:
        uint window;

        if (
            (window = Interlocked.Decrement(ref _outboundWindow)) >= MAX_REQUEST_PER_MINUTE)
        {
            // we're in the ratelimit.
            var delay = _windowResetsAtTicks - DateTimeOffset.UtcNow.Ticks;

            if (delay > 0)
            {
                var toWaitTimespan = TimeSpan.FromTicks(delay);

                _logger.LogDebug(
                    "Gateway preemptive Ratelimit: waiting {Delay} milliseconds...",
                    toWaitTimespan.TotalMilliseconds
                );

                await Task.Delay(toWaitTimespan, token);
            }

            // resets the '_outboundWindow' only if we're the last caller that's waiting, since we hold the 
            // current value of the window in the 'window' variable.
            if (window == Interlocked.CompareExchange(ref _outboundWindow, MAX_REQUEST_PER_MINUTE, window))
            {
                _logger.LogDebug("Gateway Ratelimit reset.");
            }

            // we don't have to reset the '_windowResetsAtTicks' variable here since that's handled below in 
            // the condition that someone actually wants to preform a request

            goto start;
        }

        if (window == MAX_REQUEST_PER_MINUTE - 1)
        {
            // this is the first request, we can safely set the reset at value
            _windowResetsAtTicks = DateTimeOffset.UtcNow.AddMinutes(1).Ticks;
            _logger.LogDebug("Gateway Ratelimit reset.");
        }
        else
        {
            var ticks = _windowResetsAtTicks;
            var resetsAt = ticks - DateTimeOffset.UtcNow.Ticks;

            if (
                resetsAt < 0
                &&
                Interlocked.CompareExchange(
                    ref _windowResetsAtTicks,
                    DateTimeOffset.UtcNow.AddMinutes(1).Ticks,
                    ticks
                ) == ticks
            )
            {
                _logger.LogDebug("Gateway Ratelimit reset.");
            }
        }

        _logger.LogDebug(
            "Gateway Ratelimit: {Current}/{Max} | Resets in {Seconds} seconds",
            MAX_REQUEST_PER_MINUTE - window,
            MAX_REQUEST_PER_MINUTE,
            Math.Round(TimeSpan.FromTicks(_windowResetsAtTicks - DateTimeOffset.UtcNow.Ticks).TotalSeconds, 1)
        );
    }
}