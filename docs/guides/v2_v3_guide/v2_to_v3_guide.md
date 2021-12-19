---
uid: Guides.V2V3Guide
title: V2 -> V3 Guide
---

# V2 to V3 Guide

V3 is designed to be a more feature complete, more reliable,
and more flexible library than any previous version.

Below are the most notable breaking changes that you would need to update your code to work with V3.

### ReactionAdded Event

The reaction added event has been changed to have both parameters cacheable.
This allows you to download the channel and message if they aren't cached instead of them being null.

### UserIsTyping Event

The user is typing event has been changed to have both parameters cacheable.
This allows you to download the user and channel if they aren't cached instead of them being null.

### Presence

There is a new event called `PresenceUpdated` that is called when a user's presence changes,
instead of `GuildMemberUpdated` or `UserUpdated`.
If your code relied on these events to get presence data then you need to update it to work with the new event.

## Migrating your commands to slash command

The new InteractionService was designed to act like the previous service for text-based commands.
Your pre-existing code will continue to work, but you will need to migrate your modules and response functions to use the new
InteractionService methods. Docs on this can be found in the Guides section.
