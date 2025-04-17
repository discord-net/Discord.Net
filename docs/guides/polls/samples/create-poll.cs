
// Create a poll
var poll = new PollProperties
{
    // Set the question
    Question = new ()
    {   // Text of the question
        Text = "Discord.Net is awesome!"
    },
    // Set the duration of the poll in hours
    Duration = 69,
    // Add answers to the poll
    // You can add from 1 to 10 answers
    Answers = [
        // An answer can consist of text and an emoji
        new PollMediaProperties
        {   // Text for the answer
            Text = "Yes!",
            // Emoji for the answer
            // Can be a custom emoji or unicode one
            // Remember that bot must be in the guild where the custom emoji is
            Emoji = Emote.Parse("<:wires:1214532316999974962>")
        },
        // Or just text
        new PollMediaProperties
        {
            Text = "Of course!",
        }
        ],
    // You can allow users to select multiple answers
    // By default, it's set to false
    AllowMultiselect = true,
    // Also you can set the layout of the poll
    // By default, it's set to Default
    // At this time, it's the only available layout type
    LayoutType = PollLayout.Default
};

// Send the poll to the text channel
await textChannel.SendMessageAsync(poll: poll);