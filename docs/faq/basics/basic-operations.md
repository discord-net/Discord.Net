---
uid: FAQ.Basics.BasicOp
title: Basic Operations Questions
---

# Basic Operations Questions

In the following section, you will find commonly asked questions and
answers regarding basic usage of the library, as well as
language-specific tips when using this library.

## How should I safely check a type?

> [!WARNING]
> Direct casting (e.g., `(Type)type`) is **the least recommended**
> way of casting, as it _can_ throw an [InvalidCastException]
> when the object isn't the desired type.
>
> Please refer to [this post] for more details.

In Discord.Net, the idea of polymorphism is used throughout. You may
need to cast the object as a certain type before you can perform any
action.

A good and safe casting example:

[!code-csharp[Casting](samples/cast.cs)]

[invalidcastexception]: https://docs.microsoft.com/en-us/dotnet/api/system.invalidcastexception
[this post]: https://docs.microsoft.com/en-us/dotnet/csharp/how-to/safely-cast-using-pattern-matching-is-and-as-operators

## How do I send a message?

> [!TIP]
> The [GetChannel] method by default returns an [IChannel], allowing
> channel types such as [IVoiceChannel], [ICategoryChannel]
> to be returned; consequently, you cannot send a message
> to channels like those.

Any implementation of [IMessageChannel] has a [SendMessageAsync]
method. You can get the channel via [GetChannel] under the client.
Remember, when using Discord.Net, polymorphism is a common recurring
theme. This means an object may take in many shapes or form, which
means casting is your friend. You should attempt to cast the channel
as an [IMessageChannel] or any other entity that implements it to be
able to message.

[sendmessageasync]: xref:Discord.IMessageChannel.SendMessageAsync*
[getchannel]: xref:Discord.WebSocket.DiscordSocketClient.GetChannel*

## How can I tell if a message is from X, Y, Z channel?

You may check the message channel type. Visit [Glossary] to see the
various types of channels.

[Glossary]: xref:Guides.Entities.Glossary#channels

## How can I get the guild from a message?

There are 2 ways to do this. You can do either of the following,

1. Cast the user as an [IGuildUser] and use its [IGuild] property.
2. Cast the channel as an [IGuildChannel] and use its [IGuild] property.

## How do I add hyperlink text to an embed?

Embeds can use standard [markdown] in the description field as well
as in field values. With that in mind, links can be added with
`[text](link)`.

[markdown]: https://support.discordapp.com/hc/en-us/articles/210298617-Markdown-Text-101-Chat-Formatting-Bold-Italic-Underline-

## How do I add reactions to a message?

Any entity that implements [IUserMessage] has an [AddReactionAsync]
method. This method expects an [IEmote] as a parameter.
In Discord.Net, an Emote represents a custom-image emote, while an
Emoji is a Unicode emoji (standard emoji). Both [Emoji] and [Emote]
implement [IEmote] and are valid options.

# [Adding a reaction to another message](#tab/emoji-others)

[!code-csharp[Emoji](samples/emoji-others.cs)]

# [Adding a reaction to a sent message](#tab/emoji-self)

[!code-csharp[Emoji](samples/emoji-self.cs)]

---

[addreactionasync]: xref:Discord.IMessage.AddReactionAsync*

## What is a "preemptive rate limit?"

A preemptive rate limit is Discord.Net's way of telling you to slow
down before you get hit by the real rate limit. Hitting a real rate
limit might prevent your entire client from sending any requests for
a period of time. This is calculated based on the HTTP header
returned by a Discord response.

## Why am I getting so many preemptive rate limits when I try to add more than one reactions?

This is due to how HTML header works, mistreating
0.25sec/action to 1sec. This causes the lib to throw preemptive rate
limit more frequently than it should for methods such as adding
reactions.

## Can I opt-out of preemptive rate limits?

Unfortunately, not at the moment. See [#401](https://github.com/discord-net/Discord.Net/issues/401).

[IChannel]: xref:Discord.IChannel
[ICategoryChannel]: xref:Discord.ICategoryChannel
[IGuildChannel]: xref:Discord.IGuildChannel
[ITextChannel]: xref:Discord.ITextChannel
[IGuild]: xref:Discord.IGuild
[IVoiceChannel]: xref:Discord.IVoiceChannel
[IGuildUser]: xref:Discord.IGuildUser
[IMessageChannel]: xref:Discord.IMessageChannel
[IUserMessage]: xref:Discord.IUserMessage
[IEmote]: xref:Discord.IEmote
[Emote]: xref:Discord.Emote
[Emoji]: xref:Discord.Emoji
