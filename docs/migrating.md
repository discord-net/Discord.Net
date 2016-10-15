# Migrating from 0.9

**1.0.0 is the biggest breaking change the library has gone through, due to massive
changes in the design of the library.**

>A medium to advanced understanding is recommended when working with this library.

It is recommended to familiarize yourself with the entities in 1.0 before continuing. 
Feel free to look through the library's source directly, look through IntelliSense, or 
look through our hosted [API Documentation](xref:Discord).

## Entities 

Most API models function _similarly_ to 0.9, however their names have been changed. 
You should also keep in mind that we now separate different types of Channels and Users.

Before proceeding, please read over @Terminology to understand the naming behind some objects.

Below is a table that compares most common 0.9 entities to their 1.0 counterparts.

>This should be used mostly for migration purposes. Please take some time to consider whether
>or not you are using the right "tool for the job" when working with 1.0

| 0.9 | 1.0 | Notice |
| --- | --- | ------ |
| Server | @Discord.WebSocket.SocketGuild |
| Channel | @Discord.WebSocket.SocketGuildChannel | Applies only to channels that are members of a Guild |
| Channel.IsPrivate | @Discord.WebSocket.SocketDMChannel
| ChannelType.Text | @Discord.WebSocket.SocketTextChannel | This applies only to Text Channels in Guilds
| ChannelType.Voice | @Discord.WebSocket.SocketVoiceChannel | This applies only to Voice Channels in Guilds
| User | @Discord.WebSocket.SocketGuildUser | This applies only to users belonging to a Guild*
| Profile | @Discord.WebSocket.SocketGuildUser
| Message | @Discord.WebSocket.SocketUserMessage

\* To retrieve an @Discord.WebSocket.SocketGuildUser, you must retrieve the user from an @Discord.WebSocket.SocketGuild.

## Event Registration

Prior to 1.0, events were registered using the standard c# `Handler(EventArgs)` pattern. In 1.0,
events are delegates, but are still registered the same.

For example, let's look at [DiscordSocketClient.MessageReceived](xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_MessageReceived)

To hook an event into MessageReceived, we now use the following code:  
[!code-csharp[Event Registration](guides/samples/migrating/event.cs)]

> **All Event Handlers in 1.0 MUST return Task!**

If your event handler is marked as `async`, it will automatically return `Task`. However,
if you do not need to execute asynchronus code, do _not_ mark your handler as `async`, and instead,
stick a `return Task.CompletedTask` at the bottom.

[!code-csharp[Sync Event Registration](guides/samples/migrating/sync_event.cs)]

**Event handlers no longer require a sender.** The only arguments your event handler needs to accept
are the parameters used by the event. It is recommended to look at the event in IntelliSense or on the
API docs before implementing it.

## Async

Nearly everything in 1.0 is an async Task. You should always await any tasks you invoke.
