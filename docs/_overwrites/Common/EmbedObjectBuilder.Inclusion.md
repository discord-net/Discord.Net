The example will build a rich embed with an author field, a footer
field, and 2 normal fields using an @Discord.EmbedBuilder:

```cs
var exampleAuthor = new EmbedAuthorBuilder()
        .WithName("I am a bot")
        .WithIconUrl("https://discordapp.com/assets/e05ead6e6ebc08df9291738d0aa6986d.png");
var exampleFooter = new EmbedFooterBuilder()
        .WithText("I am a nice footer")
        .WithIconUrl("https://discordapp.com/assets/28174a34e77bb5e5310ced9f95cb480b.png");
var exampleField = new EmbedFieldBuilder()
        .WithName("Title of Another Field")
        .WithValue("I am an [example](https://example.com).")
        .WithInline(true);
var otherField = new EmbedFieldBuilder()
        .WithName("Title of a Field")
        .WithValue("Notice how I'm inline with that other field next to me.")
        .WithInline(true);
var embed = new EmbedBuilder()
        .AddField(exampleField)
        .AddField(otherField)
        .WithAuthor(exampleAuthor)
        .WithFooter(exampleFooter)
        .Build();
```