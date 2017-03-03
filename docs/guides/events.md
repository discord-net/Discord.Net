---
title: Events
---

# Events

Messages from Discord are exposed via events, and follow a pattern of `Func<[event params], Task>`, which allows you to easily create either async or sync event handlers.

To hook into events, you must be using the @Discord.WebSocket.DiscordSocketClient, which provides WebSocket capabilities, necessary for receiving events.

>[!NOTE]
>The gateway will wait for all registered handlers of an event to finish before raising the next event. As a result of this, it is reccomended that if you need to perform any heavy work in an event handler, it is done on its own thread or Task.

**For further documentation of all events**, it is reccomended to look at the [Events Section](xref:Discord.WebSocket.DiscordSocketClient#events) on the API documentation of @Discord.WebSocket.DiscordSocketClient

## Connection State

Connection Events will be raised when the Connection State of your client changes.

[DiscordSocketClient.Connected](xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_Connected) and [Disconnected](xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_Disconnected) are raised when the Gateway Socket connects or disconnects, respectively.

>[!WARNING]
>You should not use DiscordClient.Connected to run code when your client first connects to Discord. The client has not received and parsed the READY event and guild stream yet, and will have an incomplete or empty cache.

[DiscordSocketClient.Ready](xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_Ready) is raised when the `READY` packet is parsed and received from Discord.

>[!NOTE]
>The [DiscordSocketClient.ConnectAsync](xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_ConnectAsync_System_Boolean_) method will not return until the READY packet has been processed. By default, it also will not return until the guild stream has finished. This means it is safe to run bot code directly after awaiting the ConnectAsync method.