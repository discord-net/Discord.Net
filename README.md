# Discord.Net v0.3.1
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

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
 
### Related Projects
- [DiscordBot](https://github.com/RogueException/DiscordBot) - A basic Discord.Net extension to add command and whitelist support.
- [discord.js](https://github.com/hydrabolt/discord.js) - Javascript/Node API wrapper for Discord
- [node-discord](https://github.com/izy521/node-discord) - Javascript/Node API wrapper for Discord
