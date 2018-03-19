# Basic Operations Questions

## How should I safely check a type?
In Discord.NET, the idea of polymorphism is used throughout. You may need to cast the object as a certain type before you can perform any action. There are several ways to cast, with direct casting `(Type)type` being the the least recommended, as it *can* throw an `InvalidCastException` when the object isn't the desired type. Please refer to [this post](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-safely-cast-by-using-as-and-is-operators) for more details.

A good and safe casting example:
```cs
public async Task MessageReceivedHandler(SocketMessage msg)
{
   // Option 1:
   // Using the `as` keyword, which will return `null` if the object isn't the desired type.
   var usermsg = msg as SocketUserMessage;
   // We bail when the message isn't the desired type.
   if (msg == null) return;
   
   // Option 2:
   // Using the `is` keyword to cast (C#7 or above only)
   if (msg is SocketUserMessage usermsg) 
   {
      // Do things
   }
}
```

## How do I send a message?

   Any implementation of **IMessageChannel** has a **SendMessageAsync** method. Using the client, you can get an appropriate channel (**GetChannel(id)**) to send a message to. Remember, when using Discord.NET, polymorphism is a common recurring theme. This means an object may take in many shapes or form, which means casting is your friend. You should attempt to cast the channel as an `IMessageChannel` or any other entity that implements it to be able to message.

## How can I tell if a message is from X, Y, Z?

   You may check message channel type.
   
   * A **Text channel** (`ITextChannel`) is a message channel from a Guild.
   * A **DM channel** (`IDMChannel`) is a message channel from a DM.
   * A **Group channel** (`IGroupChannel`) is a message channel from a Group (this is rarely used, due to the bot's inability to join a group).
   * A **Private channel** (`IPrivateChannel`) is a DM or a Group.
   * A **Message channel** (`IMessageChannel`) is all of the above.
   
## How do I add hyperlink text to an embed?

   Embeds can use standard [markdown](https://support.discordapp.com/hc/en-us/articles/210298617-Markdown-Text-101-Chat-Formatting-Bold-Italic-Underline-) in the Description as well as in field values. With that in mind, links can be added using the following format \[text](link).
   
   
## How do I add reactions to a message?

  Any entities that implement `IUserMessage` has an **AddReactionAsync** method. This method expects an `IEmote` as a parameter. In Discord.Net, an Emote represents a server custom emote, while an Emoji is a Unicode emoji (standard emoji).  Both `Emoji` and `Emote`  implement `IEmote` and are valid options. 
  ```cs
  // bail if the message is not a user one (system messages cannot have reactions)
  var usermsg = msg as IUserMessage;
  if (usermsg == null) return;
  
  // standard Unicode emojis
  Emoji emoji = new Emoji("👍");
  // or
  // Emoji emoji = new Emoji("\u23F8");
  
  // custom guild emotes
  Emote emote = Emote.Parse("<:dotnet:232902710280716288>");  
  // using Emote.TryParse may be safer in regards to errors being thrown;
  // please note that the method does not verify if the emote exists,
  // it simply creates the Emote object for you.
  
  // add the reaction to the message
  await usermsg.AddReactionAsync(emoji); 
  await usermsg.AddReactionAsync(emote); 
  ```  
  
## Why am I getting so many preemptive rate limits when I try to add more than one reactions?

   This is due to how .NET parses the HTML header, mistreating 0.25sec/action to 1sec. This casues the lib to throw preemptive rate limit more frequently than it should for methods such as adding reactions.


## Can I opt-out of preemptive rate limits?
   
   Unfortunately, not at the moment. See [#401](https://github.com/RogueException/Discord.Net/issues/401).