async Task RunAsync()
{
    //...

    await _serviceProvider.GetRequiredService<ServiceActivator>()
        .ActivateAsync();

    //...
}
