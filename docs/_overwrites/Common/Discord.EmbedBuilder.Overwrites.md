---
uid: Discord.EmbedBuilder
---

### Remarks

This builder class is used to build an @Discord.Embed (rich embed)
object that will be ready to be sent via @Discord.IMessageChannel.SendMessageAsync* 
after @Discord.EmbedBuilder.Build* is called.

### Example

```cs
public async Task SendRichEmbedAsync()
{
    var embed = new EmbedBuilder
                    {
                        // Embed property can be set within object initializer
                        Title = "Hello world!"
                    }
                    // Or with the method
                    .WithTitle("I overwrote the title.")
                    .WithDescription("I am a description.")
                    .WithUrl("https://example.com")
                    .Build();
    await _channel.SendMessageAsync(string.Empty, embed: embed);
}
```