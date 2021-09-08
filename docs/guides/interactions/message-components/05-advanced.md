# Advanced

Lets say you have some components on an ephemeral slash command, and you want to modify the message that the button is on. The issue with this is that ephemeral messages are not stored and can not be get via rest or other means.

Luckily, Discord thought of this and introduced a way to modify them with interactions.

### Using the UpdateAsync method

Components come with an `UpdateAsync` method that can update the message that the component was on. You can use it like a `ModifyAsync` method.

Lets use it with a command, we first create our command, in this example im just going to use a message command:

```cs
var command = new MessageCommandBuilder()
    .WithName("testing").Build();

await client.GetGuild(guildId).BulkOverwriteApplicationCommandAsync(new [] { command, buttonCommand });
```

Next, we listen for this command, and respond with some components when its used:

```cs
var menu = new SelectMenuBuilder()
{
    CustomId = "select-1",
    Placeholder = "Select Somthing!",
    MaxValues = 1,
    MinValues = 1,
};

menu.AddOption("Meh", "1", "Its not gaming.")
    .AddOption("Ish", "2", "Some would say that this is gaming.")
    .AddOption("Moderate", "3", "It could pass as gaming")
    .AddOption("Confirmed", "4", "We are gaming")
    .AddOption("Excellent", "5", "It is renowned as gaming nation wide", new Emoji("🔥"));

var components = new ComponentBuilder()
    .WithSelectMenu(menu);


await arg.RespondAsync("On a scale of one to five, how gaming is this?", component: componBuild(), ephemeral: true);
break;
```

Now, let's listen to the select menu executed event and add a case for `select-1`

```cs
switch (arg.Data.CustomId)
{
    case "select-1":
        var value = arg.Data.Values.First();
        var menu = new SelectMenuBuilder()
        {
            CustomId = "select-1",
            Placeholder = $"{(arg.Message.Components.First().Components.First() as SelectMenu).Options.FirstOrDefault(x => x.Value == value).Label}",
            MaxValues = 1,
            MinValues = 1,
            Disabled = true
        };

        menu.AddOption("Meh", "1", "Its not gaming.")
            .AddOption("Ish", "2", "Some would say that this is gaming.")
            .AddOption("Moderate", "3", "It could pass as gaming")
            .AddOption("Confirmed", "4", "We are gaming")
            .AddOption("Excellent", "5", "It is renowned as gaming nation wide", new Emoji("🔥"));

        // We use UpdateAsync to update the message and its original content and components.
        await arg.UpdateAsync(x =>
        {
            x.Content = $"Thank you {arg.User.Mention} for rating us {value}/5 on the gaming scale";
            x.Components = new ComponentBuilder().WithSelectMenu(menu).Build();
        });
    break;
}
```
