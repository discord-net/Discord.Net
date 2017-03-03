---
title: Getting Started
---

# Getting Started

## Requirements

Discord.Net supports logging in with all variations of Discord Accounts, however the Discord API reccomends using a `Bot Account`.

You may [register a bot account here](https://discordapp.com/developers/applications/me).

Bot accounts must be added to a server, you must use the [OAuth 2 Flow](https://discordapp.com/developers/docs/topics/oauth2#adding-bots-to-guilds) to add them to servers.

## Installation

You can install Discord.Net 1.0 from our [MyGet Feed](https://www.myget.org/feed/Packages/discord-net).

**For most users writing bots, install only `Discord.Net.WebSocket`.**

You may add the MyGet feed to Visual Studio directly from `https://www.myget.org/F/discord-net/api/v3/index.json`.

You can also pull the latest source from [GitHub](https://github.com/RogueException/Discord.Net).

>[!WARNING]
>The versions of Discord.Net on NuGet are behind the versions this 
>documentation is written for.
>You MUST install from MyGet or Source!

## Async

Discord.Net uses C# tasks extensiely - nearly all operations return 
one. 

It is highly reccomended these tasks be awaited whenever possible. 
To do so requires the calling method to be marked as async, which 
can be problematic in a console application. An example of how to 
get around this is provided below.

For more information, go to [MSDN's Async-Await section.](https://msdn.microsoft.com/en-us/library/hh191443.aspx)

## First Steps

[!code-csharp[Main](samples/first-steps.cs)]

>[!NOTE]
>In previous versions of Discord.Net, you had to hook into the `Ready` and `GuildAvailable` events to determine when your client was ready for use. 
>In 1.0, the [ConnectAsync] method will automatically wait for the Ready event, and for all guilds to stream. To avoid this, pass `false` into `ConnectAsync`. 

[ConnectAsync]: xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_ConnectAsync_System_Boolean_