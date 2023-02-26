---
uid: Guides.GuildEvents.Intro
title: Introduction to Guild Events
---

# Guild Events

Guild events are a way to host events within a guild. They offer a lot of features and flexibility.

## Getting started with guild events

You can access any events within a guild by calling `GetEventsAsync` on a guild.

```cs
var guildEvents = await guild.GetEventsAsync();
```

If your working with socket guilds you can just use the `Events` property:

```cs
var guildEvents = guild.Events;
```

There are also new gateway events that you can hook to receive guild scheduled events on.

```cs
// Fired when a guild event is cancelled.
client.GuildScheduledEventCancelled += ...

// Fired when a guild event is completed.
client.GuildScheduledEventCompleted += ...

// Fired when a guild event is started.
client.GuildScheduledEventStarted += ...

// Fired when a guild event is created.
client.GuildScheduledEventCreated += ...

// Fired when a guild event is updated.
client.GuildScheduledEventUpdated += ...
```
