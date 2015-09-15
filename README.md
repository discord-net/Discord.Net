# Discord.Net v0.7.0-beta1
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

[Join the discussion](https://discord.gg/0SBTUU1wZTVjAMPx) on Discord.

### This is an alpha!
The Discord API is still in active development, meaning this library may break at any time without notice.
Discord.Net itself is also in alpha so several functions may be unstable or not work at all. 

### Features
- Server Management (Servers, Channels, Messages, Invites)
- User Moderation (Kick/Ban/Unban/Mute/Unmute/Deafen/Undeafen)
- Alpha Voice Support (Outgoing only currently)
- Supports .Net 4.5, DNX 4.5.1 and DNX Core 5.0 (Windows only)

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
await client.AcceptInvite("channel-invite-code");
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
await client.AcceptInvite("channel-invite-code");

```

### Known Issues
- Due to current Discord restrictions, private messages are blocked unless both the sender and recipient are members of the same server.
- Caches do not currently clean up when their entries are no longer referenced, and there is no cap to the message cache. For now, disconencting and reconnecting will clear all caches.
