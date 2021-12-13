// bail if the message is not a user one (system messages cannot have reactions)
var usermsg = msg as IUserMessage;
if (usermsg == null) return;

// standard Unicode emojis
Emoji emoji = new Emoji("👍");
// or
// Emoji emoji = new Emoji("\uD83D\uDC4D");

// custom guild emotes
Emote emote = Emote.Parse("<:dotnet:232902710280716288>");  
// using Emote.TryParse may be safer in regards to errors being thrown;
// please note that the method does not verify if the emote exists,
// it simply creates the Emote object for you.

// add the reaction to the message
await usermsg.AddReactionAsync(emoji); 
await usermsg.AddReactionAsync(emote); 