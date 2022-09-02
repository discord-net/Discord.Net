public class ServiceActivator
{
    // This contains *all* registered services of serviceType IService
    private readonly IEnumerable<IService> _services;

    public ServiceActivator(IEnumerable<IService> services)
    {
        _services = services;
    }

    public async Task ActivateAsync()
    {
        foreach(var service in _services)
        {
            await service.StartAsync();
        }
    }
}
