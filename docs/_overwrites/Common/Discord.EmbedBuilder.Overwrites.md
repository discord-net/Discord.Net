---
uid: Discord.EmbedBuilder
remarks: *content
---

This builder class is used to build an @Discord.Embed (rich embed)
object that will be ready to be sent via @Discord.IMessageChannel.SendMessageAsync*
after @Discord.EmbedBuilder.Build* is called.

---
uid: Discord.EmbedBuilder
example: [*content]
---

The example below builds an embed and sends it to the chat.

```cs
[Command("embed")]
public async Task SendRichEmbedAsync()
{
    var embed = new EmbedBuilder
        {
            // Embed property can be set within object initializer
            Title = "Hello world!"
        }
        // Or with methods
        .AddField("Field title",
        "Field value. I also support [hyperlink markdown](https://example.com)!")
        .WithAuthor(Context.Client.CurrentUser)
        .WithFooter(footer => footer.Text = "I am a footer.")
        .WithColor(Color.Blue)
        .WithTitle("I overwrote \"Hello world!\"")
        .WithDescription("I am a description.")
        .WithUrl("https://example.com")
        .WithCurrentTimestamp()
        .Build();
    await ReplyAsync(embed: embed);
}
```

#### Result

![Embed Example](images/embed-example.png)