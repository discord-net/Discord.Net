# Discord.Net v0.7.1-beta1
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

[Join the discussion](https://discord.gg/0SBTUU1wZTVjAMPx) on Discord.

### This is an alpha!
The Discord API is still in active development, meaning this library may break at any time without notice.
Discord.Net itself is also in alpha so several functions may be unstable or not work at all. 

### Current Features
- Using Discord API version 3
- Supports .Net 4.5 and DNX 4.5.1
- Server Management (Servers, Channels, Messages, Invites, Roles, Users)
- Send/Receieve Messages (Including mentions and formatting)
- Basic Voice Support (Outgoing only, Unencrypted only)
- Command extension library (Supports permission levels)

### NuGet Packages
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

### Example (Echo Client)
```
var client = new DiscordClient();
client.MessageCreated += async (s, e) =>
{
	if (e.Message.UserId != client.User.Id)
		await client.SendMessage(e.Message.ChannelId, e.Message.Text);
};
await client.Connect("discordtest@email.com", "Password123");
```

### Example (Command Client)
(Requires Discord.Net.Commands)
```
var client = new DiscordBotClient();
client.CreateCommand("acceptinvite")
	.ArgsEqual(1)
	.Do(async e =>
	{
		try
		{
			await _client.AcceptInvite(e.Args[0]);
			await _client.SendMessage(e.Channel, $"Invite \"{e.Args[0]}\" accepted.");
		}
		catch (HttpException ex)
		{
			await _client.SendMessage(e.Channel, $"Error: {ex.Message}");
		}
	});
await client.Connect("discordtest@email.com", "Password123");
```

### Known Issues
- Due to current Discord restrictions, private messages are blocked unless both the sender and recipient are members of the same server.
- The Message caches does not currently clean up when their entries are no longer referenced, and there is currently no cap to it. For now, disconnecting and reconnecting will clear all caches.
- DNX Core 5.0 is experiencing several network-related issues and support has been temporarily dropped.
