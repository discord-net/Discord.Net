---
uid: Guides.Entities.Intro
title: Introduction
---

# Entities in Discord.Net

Discord.Net provides a versatile entity system for navigating the
Discord API.

> [!TIP]
> It is **vital** that you use the proper IDs for an entity when using
> a `GetXXX` method. It is recommended that you enable Discord's
> _developer mode_ to allow easy access to entity IDs, found in
> Settings > Appearance > Advanced. Read more about it in the
> [FAQ](xref:FAQ.Basics.GetStarted) page.

## Inheritance

Due to the nature of the Discord API, some entities are designed with
multiple variants; for example, `IUser` and `IGuildUser`.

All models will contain the most detailed version of an entity
possible, even if the type is less detailed.

## Socket & REST

REST entities are retrieved over REST, and will be disposed after use.
It is suggested to limit the amount of REST calls as much as possible,
as calls over REST interact with the API, and are thus prone to rate-limits.

- [Learn more about REST](https://restfulapi.net/)

Socket entities are created through the gateway,
most commonly through `DiscordSocketClient` events.
These entities will enter the clients' global cache for later use.

In the case of the `MessageReceived` event, a
`SocketMessage` is passed in with a channel property of type
`SocketMessageChannel`. All messages come from channels capable of
messaging, so this is the only variant of a channel that can cover
every single case.

But that doesn't mean a message _can't_ come from a
`SocketTextChannel`, which is a message channel in a guild. To
retrieve information about a guild from a message entity, you will
need to cast its channel object to a `SocketTextChannel`.

> [!NOTE]
> You can find out the inheritance tree & definitions of various entities
> [here](xref:Guides.Entities.Glossary)

## Navigation

All socket entities have navigation properties on them, which allow
you to easily navigate to an entity's parent or children. As explained
above, you will sometimes need to cast to a more detailed version of
an entity to navigate to its parent.

## Accessing Socket Entities

The most basic forms of entities, `SocketGuild`, `SocketUser`, and
`SocketChannel` can be pulled from the DiscordSocketClient's global
cache, and can be retrieved using the respective `GetXXX` method on
DiscordSocketClient.

More detailed versions of entities can be pulled from the basic
entities, e.g., `SocketGuild.GetUser`, which returns a
`SocketGuildUser`, or `SocketGuild.GetChannel`, which returns a
`SocketGuildChannel`. Again, you may need to cast these objects to get
a variant of the type that you need.

### Sample

[!code-csharp[Socket Sample](samples/socketentities.cs)]

## Accessing REST Entities

REST entities work almost the same as Socket entities, but are much less frequently used.
To access REST entities, the `DiscordSocketClient`'s `Rest` property is required.
Another option here is to create your own [DiscordRestClient], independent of the Socket gateway.

[DiscordRestClient]: xref:Discord.Rest.DiscordRestClient

### Sample

[!code-csharp[Rest Sample](samples/restentities.cs)]
