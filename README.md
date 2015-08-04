# Discord.Net
A .Net API Wrapper for the Discord client (http://discordapp.com).

## This is an alpha build!
The Discord API is still in active development, meaning this library may break at any time without notice.
Discord.Net is also in early development so several functions may be unstable or not work at all.

# Features
- Login/Logout (account or anonymous)
- Accepting Invites (standard or human readable)
- Deleting Invites
- Receiving/Sending Messages
- Creating/Destroying Servers
- Creating/Destroying Channels
- Several Discord Events

# Upcoming
- Modifying User/Channel/Server Settings
- Creating Invites
- Kick/Ban/Unban/Mute/Unmute/Deafen/Undeafen
- Sending Private Messages

# Example (Echo Client)
```
var client = new DiscordClient();
client.MessageCreated += (s, e) =>
{
	client.SendMessage(e.Message.ChannelId, e.Message.Text);
};
await client.Connect("discordtest@email.com", "Password123");
await client.AcceptInvite("channel-invite-code");
```

# Remarks

Due to current Discord restrictions, the client is unable to private message users unless both they and the client's account are members of the same server.
