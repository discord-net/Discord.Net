// capture the message you're sending in a variable
var msg = await channel.SendMessageAsync("This will have reactions added.");

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
await msg.AddReactionAsync(emoji); 
await msg.AddReactionAsync(emote); 