---
uid: Guides.Concepts.ManageConnections
title: Managing Connections
---

# Managing Connections with Discord.Net

In Discord.Net, once a client has been started, it will automatically
maintain a connection to Discord's gateway until it is manually
stopped.

## Usage

To start a connection, invoke the `StartAsync` method on a client that
supports a WebSocket connection; to end a connection, invoke the
`StopAsync` method, which gracefully closes any open WebSocket or
UdpSocket connections.

Since the Start/Stop methods only signal to an underlying connection
manager that a connection needs to be started, **they return before a
connection is made.**

As a result, you need to hook into one of the connection-state
based events to have an accurate representation of when a client is
ready for use.

All clients provide a `Connected` and `Disconnected` event, which is
raised respectively when a connection opens or closes. In the case of
the [DiscordSocketClient], this does **not** mean that the client is
ready to be used.

A separate event, `Ready`, is provided on [DiscordSocketClient], which
is raised only when the client has finished guild stream or guild
sync and has a completed guild cache.

[DiscordSocketClient]: xref:Discord.WebSocket.DiscordSocketClient

### Samples

[!code-csharp[Connection Sample](samples/events.cs)]

## Reconnection

> [!TIP]
> Avoid running long-running code on the gateway! If you deadlock the
> gateway (as explained in [events]), the connection manager will
> **NOT** be able to recover and reconnect.

Assuming the client disconnected because of a fault on Discord's end,
and not a deadlock on your end, we will always attempt to reconnect
and resume a connection.

Don't worry about trying to maintain your own connections, the
connection manager is designed to be bulletproof and never fail - if
your client does not manage to reconnect, you have found a bug!

[events]: xref:Guides.Concepts.Events
