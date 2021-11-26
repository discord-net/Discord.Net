---
uid: Guides.Concepts.Entities
title: Entities
---

# Entities in Discord.Net

> [!NOTE]
> This article is written with the Socket variants of entities in mind,
> not the general interfaces or Rest entities.

Discord.Net provides a versatile entity system for navigating the
Discord API.

## Inheritance

Due to the nature of the Discord API, some entities are designed with
multiple variants; for example, `SocketUser` and `SocketGuildUser`.

All models will contain the most detailed version of an entity
possible, even if the type is less detailed.

For example, in the case of the `MessageReceived` event, a
`SocketMessage` is passed in with a channel property of type
`SocketMessageChannel`. All messages come from channels capable of
messaging, so this is the only variant of a channel that can cover
every single case.

But that doesn't mean a message _can't_ come from a
`SocketTextChannel`, which is a message channel in a guild. To
retrieve information about a guild from a message entity, you will
need to cast its channel object to a `SocketTextChannel`.

You can find out various types of entities in the @FAQ.Misc.Glossary
page.

## Navigation

All socket entities have navigation properties on them, which allow
you to easily navigate to an entity's parent or children. As explained
above, you will sometimes need to cast to a more detailed version of
an entity to navigate to its parent.

## Accessing Entities

The most basic forms of entities, `SocketGuild`, `SocketUser`, and
`SocketChannel` can be pulled from the DiscordSocketClient's global
cache, and can be retrieved using the respective `GetXXX` method on
DiscordSocketClient.

> [!TIP]
> It is **vital** that you use the proper IDs for an entity when using
> a `GetXXX` method. It is recommended that you enable Discord's
> _developer mode_ to allow easy access to entity IDs, found in
> Settings > Appearance > Advanced. Read more about it in the
> [FAQ](xref:FAQ.Basics.GetStarted) page.

More detailed versions of entities can be pulled from the basic
entities, e.g., `SocketGuild.GetUser`, which returns a
`SocketGuildUser`, or `SocketGuild.GetChannel`, which returns a
`SocketGuildChannel`. Again, you may need to cast these objects to get
a variant of the type that you need.

## Sample

[!code-csharp[Entity Sample](samples/entities.cs)]