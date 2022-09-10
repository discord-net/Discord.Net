// Program.cs

var listener = services.GetRequiredService<DiscordEventListener>();
await listener.StartAsync();
