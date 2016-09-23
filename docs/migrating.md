# Migrating from 0.9

**1.0.0 is the biggest breaking change the library has gone through, due to massive
changes in the design of the library.**

>A medium to advanced understanding is recommended when working with this library.

One of the biggest major changes from `0.9.x` is the exclusive use of interfaces.
For the most part, your usability will be very similar to the 0.9 approach of concrete
classes. You **will** be required to cast some entities; this is outlined in a later
section.

It is recommended to familiarize yourself with the entities in 1.0 before continuing. 
Feel free to look through the library's source directly, look through IntelliSense, or 
look through our hosted [API Documentation](xref:Discord).

## Entities 

Most API models function _similarly_ to 0.9, however their names have been changed. 
You should also keep in mind that we now separate different types of Channels and Users.

Take a look at inheritance section of @Terminology for an example of how inheritance and interfaces
work in 1.0

Below is a table that compares most common 0.9 entities to their 1.0 counterparts.

>This should be used mostly for migration purposes. Please take some time to consider whether
>or not you are using the right "tool for the job" when working with 1.0

| 0.9 | 1.0 | Notice |
| --- | --- | ------ |
| Server | @Discord.IGuild |
| Channel | @Discord.IGuildChannel | Applies only to channels that are members of a Guild |
| Channel.IsPrivate | @Discord.IDMChannel
| ChannelType.Text | @Discord.ITextChannel | This applies only to Text Channels in Guilds
| ChannelType.Voice | @Discord.IVoiceChannel | This applies only to Voice Channels in Guilds
| User | @Discord.IGuildUser | This applies only to users belonging to a Guild*
| Profile | @Discord.ISelfUser
| Message | @Discord.IUserMessage

\* To retrieve an @Discord.IGuildUser, you must retrieve the user from an @Discord.IGuild.
[IDiscordClient.GetUserAsync](xref:Discord.IDiscordClient#Discord_IDiscordClient_GetUserAsync_System_UInt64_) 
returns a @Discord.IUser, which only contains the information that Discord exposes for public users.

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

However, when using WebSockets, you may find this both inconvienent, and unnecessary, as many of the
WebSocket implementations of the interfaces keep their own local cache of objects, rendering the use
of async redundant. 

**As of right now,** there are extension methods you can use, located in `Discord.WebSocket` that will
provide java-esque, synchronus `GetXXX` methods to replace the asynchronus methods on WebSocket entities.

This functionality may be changed at a later date, we are currently reviewing this implementation and
alternative methods.

For your reference, you may want to look through the extension classes located in @Discord.WebSocket
