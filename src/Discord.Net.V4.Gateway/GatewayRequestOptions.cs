namespace Discord.Gateway;

public sealed class GatewayRequestOptions : RequestOptions
{
    public bool UpdateCache { get; set; }

    public GatewayRequestOptions()
    { }

    public GatewayRequestOptions(RequestOptions options)
    {
        ApplyRestOptions(options);
    }

    internal GatewayRequestOptions WithRestOptions(RequestOptions options)
    {
        var gatewayOptions = (GatewayRequestOptions)MemberwiseClone();

        gatewayOptions.ApplyRestOptions(gatewayOptions);

        return gatewayOptions;
    }

    private void ApplyRestOptions(RequestOptions options)
    {
        Timeout = options.Timeout;
        RetryMode = options.RetryMode;
        AllowCached = options.AllowCached;
        AuditLogReason = options.AuditLogReason;
        UseSystemClock = options.UseSystemClock;
    }

    public static GatewayRequestOptions? FromRestOptions(RequestOptions? options)
    {
        if (options is null)
            return null;

        if (options is GatewayRequestOptions gatewayOptions)
            return gatewayOptions;

        return new GatewayRequestOptions(options);
    }
}
