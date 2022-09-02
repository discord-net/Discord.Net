static IServiceProvider CreateServices()
{
    var config = new DiscordSocketConfig()
    {
        //...
    };

    var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>();

    return collection.BuildServiceProvider();
}
