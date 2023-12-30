namespace Discord;

internal sealed class RatelimitContract(RateLimiter limiter, Bucket bucket, Action<RatelimitContract> onResolve)
{
    public bool IsActive { get; private set; }

    // don't ask, just accept
    public Task<IDisposable> EnterSuperCriticalPhaseDodalooAsync(CancellationToken token = default)
        => limiter.GetRequestHandleAsync(token);

    public void Complete(in RatelimitInfo info)
    {
        if (!IsActive)
            return;

        IsActive = false;
        bucket.Update(in info);
        onResolve(this);
    }

    public void Cancel()
    {
        if (!IsActive)
            return;

        IsActive = false;
        onResolve(this);
    }


}
