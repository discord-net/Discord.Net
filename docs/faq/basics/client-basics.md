---
uid: FAQ.Basics.ClientBasics
title: Basic Questions about Client
---

# Client Basics Questions

In the following section, you will find commonly asked questions and
answers about common issues that you may face when utilizing the
various clients offered by the library.

## My client keeps returning 401 upon logging in!

> [!WARNING]
> Userbot/selfbot (logging in with a user token) is no
> longer supported with this library starting from 2.0, as
> logging in under a user account may result in account termination.
>
> For more information, see issue [827] & [958], as well as the official
> [Discord API Terms of Service].

There are few possible reasons why this may occur.

1. You are not using the appropriate [TokenType]. If you are using a
 bot account created from the Discord Developer portal, you should
 be using `TokenType.Bot`.
2. You are not using the correct login credentials. Please keep in
 mind that a token is **different** from a *client secret*.

[TokenType]: xref:Discord.TokenType
[827]: https://github.com/RogueException/Discord.Net/issues/827
[958]: https://github.com/RogueException/Discord.Net/issues/958
[Discord API Terms of Service]: https://discord.com/developers/docs/legal

## How do I do X, Y, Z when my bot connects/logs on? Why do I get a `NullReferenceException` upon calling any client methods after connect?

Your bot should **not** attempt to interact in any way with
guilds/servers until the [Ready] event fires. When the bot
connects, it first has to download guild information from
Discord for you to get access to any server
information; the client is not ready at this point.

Technically, the [GuildAvailable] event fires once the data for a
particular guild has downloaded; however, it is best to wait for all
guilds to be downloaded. Once all downloads are complete, the [Ready]
event is triggered, then you can proceed to do whatever you like.

[Ready]: xref:Discord.WebSocket.DiscordSocketClient.Ready
[GuildAvailable]: xref:Discord.WebSocket.BaseSocketClient.GuildAvailable

## How do I get a message's previous content when that message is edited?

If you need to do anything with messages (e.g., checking Reactions,
checking the content of edited/deleted messages), you must set the
[MessageCacheSize] in your [DiscordSocketConfig] settings in order to
use the cached message entity. Read more about it [here](xref:Guides.Concepts.Events#cacheable).

1. Message Cache must be enabled.
2. Hook the MessageUpdated event. This event provides a *before* and
 *after* object.
3. Only messages received *after* the bot comes online will be
 available in the cache.

[MessageCacheSize]: xref:Discord.WebSocket.DiscordSocketConfig.MessageCacheSize
[DiscordSocketConfig]: xref:Discord.WebSocket.DiscordSocketConfig
[MessageUpdated]: xref:Discord.WebSocket.BaseSocketClient.MessageUpdated

## What is a shard/sharded client, and how is it different from the `DiscordSocketClient`?
As your bot grows in popularity, it is recommended that you should section your bot off into separate processes.
The [DiscordShardedClient] is essentially a class that allows you to easily create and manage multiple [DiscordSocketClient]
instances, with each one serving a different amount of guilds.

There are very few differences from the [DiscordSocketClient] class, and it is very straightforward
to modify your existing code to use a [DiscordShardedClient] when necessary.

1. You need to specify the total amount of shards, or shard ids, via [DiscordShardedClient]'s constructors.
2. The [Connected], [Disconnected], [Ready], and [LatencyUpdated] events
 are replaced with [ShardConnected], [ShardDisconnected], [ShardReady], and [ShardLatencyUpdated].
3. Every event handler you apply/remove to the [DiscordShardedClient] is applied/removed to each shard.
 If you wish to control a specific shard's events, you can access an individual shard through the `Shards` property.

If you do not wish to use the [DiscordShardedClient] and instead reuse the same [DiscordSocketClient] code and manually shard them,
you can do so by specifying the [ShardId] for the [DiscordSocketConfig] and pass that to the [DiscordSocketClient]'s constructor.

[DiscordSocketClient]: xref:Discord.WebSocket.DiscordSocketClient
[DiscordShardedClient]: xref:Discord.WebSocket.DiscordShardedClient
[Connected]: xref:Discord.WebSocket.DiscordSocketClient.Connected
[Disconnected]: xref:Discord.WebSocket.DiscordSocketClient.Disconnected
[LatencyUpdated]: xref:Discord.WebSocket.DiscordSocketClient.LatencyUpdated
[ShardConnected]: xref:Discord.WebSocket.DiscordShardedClient.ShardConnected
[ShardDisconnected]: xref:Discord.WebSocket.DiscordShardedClient.ShardDisconnected
[ShardReady]: xref:Discord.WebSocket.DiscordShardedClient.ShardReady
[ShardLatencyUpdated]: xref:Discord.WebSocket.DiscordShardedClient.ShardLatencyUpdated
[ShardId]: xref:Discord.WebSocket.DiscordSocketConfig.ShardId
