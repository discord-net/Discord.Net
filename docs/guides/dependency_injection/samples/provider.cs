public class UtilizingProvider
{
    private readonly IServiceProvider _provider;
    private readonly AnyService _service;

    // This service is allowed to be null because it is only populated if the service is actually available in the provider.
    private readonly AnyOtherService? _otherService;

    // This constructor injects only the service provider,
    // and uses it to populate the other dependencies.
    public UtilizingProvider(IServiceProvider provider)
    {
        _provider = provider;
        _service = provider.GetRequiredService<AnyService>();
        _otherService = provider.GetService<AnyOtherService>();
    }

    // This constructor injects the service provider, and AnyService,
    // making sure that AnyService is not null without having to call GetRequiredService
    public UtilizingProvider(IServiceProvider provider, AnyService service)
    {
        _provider = provider;
        _service = service;
        _otherService = provider.GetService<AnyOtherService>();
    }
}
