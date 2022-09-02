async Task RunAsync(string[] args)
{
    // Request the instance from the client.
    // Because we're requesting it here first, its targetted constructor will be called and we will receive an active instance.
    var client = _services.GetRequiredService<DiscordSocketClient>();

    client.Log += async (msg) =>
    {
        await Task.CompletedTask;
        Console.WriteLine(msg);
    }

    await client.LoginAsync(TokenType.Bot, "");
    await client.StartAsync();

    await Task.Delay(Timeout.Infinite);
}
