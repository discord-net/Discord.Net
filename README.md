# Discord.Net v0.4.2
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

[Join the discussion](https://discord.gg/0SBTUU1wZTVjAMPx) on Discord.

### This is an alpha!
The Discord API is still in active development, meaning this library may break at any time without notice.
Discord.Net itself is also in early development so several functions may be unstable or not work at all.

### Features
- Login/Logout (with credentials or anonymous)
- Accepting/Creating/Deleting Invites (standard or human readable)
- Receiving/Sending Messages
- Creating/Destroying Servers
- Creating/Destroying Channels (text, void or PM)
- Kick/Ban/Unban/Mute/Unmute/Deafen/Undeafen Users
- Several Discord Events
- Supports .Net 4.5, DNX 4.5.1, and DNX Core 5.0

### NuGet Packages
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

### Example (Echo Client)
```
var client = new DiscordClient();
client.MessageCreated += (s, e) =>
{
	client.SendMessage(e.Message.ChannelId, e.Message.Text);
};
await client.Connect("discordtest@email.com", "Password123");
await client.AcceptInvite("channel-invite-code");
```

### Known Issues
- Due to current Discord restrictions, private messages are blocked unless both the sender and recipient are members of the same server.
- Caches do not currently clean up when their entries are no longer referenced, and there is no cap to the message cache. For now, disconencting and reconnecting will clear all caches.
