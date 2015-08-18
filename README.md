# Discord.Net v0.4.0
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

Join the discussion for this library and other API wrappers at https://discord.gg/0SBTUU1wZTV9JAsL.

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
- Supports both .Net 4.5, DNX 4.5, and DNX Core 5.0

### Upcoming
- Modifying User/Channel/Server Settings

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
