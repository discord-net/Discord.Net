# Discord.Net v0.7.3-beta2
An unofficial .Net API Wrapper for the Discord client (http://discordapp.com).

Check out the [documentation](https://discordnet.readthedocs.org/en/latest/) or join the [Discord API Chat](https://discord.gg/0SBTUU1wZTVjAMPx).

### Current Features
- Using Discord API version 3
- Supports .Net 4.5 and DNX 4.5.1
- Server Management (Servers, Channels, Messages, Invites, Roles, Users)
- Send/Receieve Messages (Including mentions and formatting)
- Basic Voice Support (Outgoing only, Unencrypted only)
- Command extension library (Supports permission levels)

### Installation
You can download Discord.Net from NuGet:
- [Discord.Net](https://www.nuget.org/packages/Discord.Net/)
- [Discord.Net.Commands](https://www.nuget.org/packages/Discord.Net.Commands/)

### Known Issues
- Due to current Discord restrictions, private messages are blocked unless both the sender and recipient are members of the same server.
- The Message cache does not currently clean up when their entries are no longer referenced, and there is currently no cap to it. For now, disconnecting and reconnecting will clear all caches.
- DNX Core 5.0 is experiencing several network-related issues and support has been temporarily dropped.
