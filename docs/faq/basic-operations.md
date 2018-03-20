# Basic Operations Questions

## How should I safely check a type?
In Discord.NET, the idea of polymorphism is used throughout. You may 
need to cast the object as a certain type before you can perform any 
action. There are several ways to cast, with direct casting 
`(Type)type` being the the least recommended, as it *can* throw an 
[InvalidCastException] when the object isn't the desired type. 
Please refer to [this post] for more details.

A good and safe casting example:

[!code-csharp[Casting](samples/basics/cast.cs)]

[InvalidCastException]: https://docs.microsoft.com/en-us/dotnet/api/system.invalidcastexception
[this post]: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-safely-cast-by-using-as-and-is-operators

## How do I send a message?

Any implementation of [IMessageChannel] has a [SendMessageAsync]
method. You can get the channel via [GetChannel] under the client.
Remember, when using Discord.NET, polymorphism is a common recurring 
theme. This means an object may take in many shapes or form, which 
means casting is your friend. You should attempt to cast the channel 
as an [IMessageChannel] or any other entity that implements it to be 
able to message.

[SendMessageAsync]: xref:Discord.IMessageChannel#Discord_IMessageChannel_SendMessageAsync_System_String_System_Boolean_Discord_Embed_Discord_RequestOptions_
[GetChannel]: xref:Discord.WebSocket.DiscordSocketClient#Discord_WebSocket_DiscordSocketClient_GetChannel_System_UInt64_

## How can I tell if a message is from X, Y, Z channel?

You may check the message channel type. Visit [Glossary] to see the 
various types of channels.

[Glossary]: Glossary.md#message-channels

## How can I get the guild from a message?

There are 2 ways to do this. You can do either of the following,
	1. Cast the user as an [IGuildUser] and use its [IGuild] property.
	2. Cast the channel as an [ITextChannel]/[IVoiceChannel] and use 
	its [IGuild] property.

## How do I add hyperlink text to an embed?

Embeds can use standard [markdown] in the description field as well 
as in field values. With that in mind, links can be added with 
`[text](link)`.

[markdown]: https://support.discordapp.com/hc/en-us/articles/210298617-Markdown-Text-101-Chat-Formatting-Bold-Italic-Underline-

## How do I add reactions to a message?

Any entities that implement [IUserMessage] has an [AddReactionAsync]
method. This method expects an [IEmote] as a parameter. 
In Discord.Net, an Emote represents a server custom emote, while an 
Emoji is a Unicode emoji (standard emoji). Both [Emoji] and [Emote] 
implement [IEmote] and are valid options. 

[!code-csharp[Emoji](samples/basics/emoji.cs)]

[AddReactionAsync]: xref:Discord.IUserMessage#Discord_IUserMessage_AddReactionAsync_Discord_IEmote_Discord_RequestOptions_
  
## Why am I getting so many preemptive rate limits when I try to add more than one reactions?

This is due to how .NET parses the HTML header, mistreating 
0.25sec/action to 1sec. This casues the lib to throw preemptive rate 
limit more frequently than it should for methods such as adding 
reactions.

## Can I opt-out of preemptive rate limits?
   
Unfortunately, not at the moment. See [#401](https://github.com/RogueException/Discord.Net/issues/401).
   

[ITextChannel]: xref:Discord.ITextChannel
[IGuild]: xref:Discord.IGuild
[IVoiceChannel]: xref:Discord.IVoiceChannel
[IGuildUser]: xref:Discord.IGuildUser
[IMessageChannel]: xref:Discord.IMessageChannel
[IUserMessage]: xref:Discord.IUserMessage
[IEmote]: xref:Discord.IEmote
[Emote]: xref:Discord.Emote
[Emoji]: xref:Discord.Emoji