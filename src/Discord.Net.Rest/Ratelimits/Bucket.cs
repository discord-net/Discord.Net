namespace Discord.Rest.Ratelimits;

public sealed class Bucket
{
    public BucketId Id { get; }
    public string? DiscordHash { get; private set; }

    public int? Limit { get; private set; }
    public int? Remaining { get; private set; }

    public DateTimeOffset? ResetAt { get; private set; }

    private int _window;
    
    private readonly Lock _windowLock = new();
    private readonly RateLimiter _rateLimiter;
    
    public Bucket(BucketId id, RateLimiter rateLimiter)
    {
        Id = id;
        _rateLimiter = rateLimiter;

        // by default, we start with 10 requests
        _window = 10;
    }

    public async ValueTask WaitAsync(CancellationToken token)
    {
        var slot = Interlocked.Decrement(ref _window);
        
        // if we've exhausted the amount of requests we can preform, we'll wait
        if (slot < 0)
        {
            
        }
    }

    public void Update(HttpResponseMessage message)
    {
        
    }
}