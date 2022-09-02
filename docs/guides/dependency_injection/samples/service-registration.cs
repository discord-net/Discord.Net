static IServiceProvider CreateServices()
{
    var config = new DiscordSocketConfig()
    {
        //...
    };

    // X represents either Interaction or Command, as it functions the exact same for both types.
    var servConfig = new XServiceConfig()
    {
        //...
    }

    var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton(servConfig)
        .AddSingleton<XService>();

    return collection.BuildServiceProvider();
}
