---
uid: Guides.V2V3Guide
title: V2 -> V3 Guide
---

# V2 to V3 Guide

V3 is designed to be a more feature complete, more reliable,
and more flexible library than any previous version.

Below are the most notable breaking changes that you would need to update your code to work with V3.

### GatewayIntents

As Discord.NET has upgraded from Discord API v6 to API v9,
`GatewayIntents` must now be specified in the socket config, as well as on the [developer portal].

```cs

// Where ever you declared your websocket client.
DiscordSocketClient _client;

...

var config = new DiscordSocketConfig()
{
  .. // Other config options can be presented here.
  GatewayIntents = GatewayIntents.All
}

_client = new DiscordSocketClient(config);

```

#### Common intents:

- AllUnprivileged: This is a group of most common intents, that do NOT require any [developer portal] intents to be enabled.
  This includes intents that receive messages such as: `GatewayIntents.GuildMessages, GatewayIntents.DirectMessages`
- GuildMembers: An intent disabled by default, as you need to enable it in the [developer portal].
- GuildPresences: Also disabled by default, this intent together with `GuildMembers` are the only intents not included in `AllUnprivileged`.
- All: All intents, it is ill advised to use this without care, as it _can_ cause a memory leak from presence.
  The library will give responsive warnings if you specify unnecessary intents.

> [!NOTE]
> All gateway intents, their Discord API counterpart and their enum value are listed
> [HERE](xref:Discord.GatewayIntents)

#### Stacking intents:

It is common that you require several intents together.
The example below shows how this can be done.

```cs

GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | ..

```

> [!NOTE]
> Further documentation on the ` | ` operator can be found
> [HERE](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators)

[developer portal]: https://discord.com/developers/

### UserLeft event

UserLeft has been changed to have the `SocketUser` and `SocketGuild` parameters instead of a `SocketGuildUser` parameter.
Because of this, guild-only user data cannot be retrieved from this user anymore, as this user is not part of the guild.

### ReactionAdded event

The reaction added event has been changed to have both parameters cacheable.
This allows you to download the channel and message if they aren't cached instead of them being null.

### UserIsTyping Event

The user is typing event has been changed to have both parameters cacheable.
This allows you to download the user and channel if they aren't cached instead of them being null.

### Presence

There is a new event called `PresenceUpdated` that is called when a user's presence changes,
instead of `GuildMemberUpdated` or `UserUpdated`.
If your code relied on these events to get presence data then you need to update it to work with the new event.

## Migrating your commands to application commands

The new interaction service was designed to act like the previous service for text-based commands.
Your pre-existing code will continue to work, but you will need to migrate your modules and response functions to use the new
interaction service methods. Documentation on this can be found in the [Guides](xref:Guides.IntFw.Intro).
