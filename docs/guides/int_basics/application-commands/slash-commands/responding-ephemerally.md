---
uid: Guides.SlashCommands.Ephemeral
title: Ephemeral Responses
---

# Responding ephemerally

What is an ephemeral response? Basically, only the user who executed the command can see the result of it, this is pretty simple to implement.

> [!NOTE]
> You don't have to run arg.DeferAsync() to capture the interaction, you can use arg.RespondAsync() with a message to capture it, this also follows the ephemeral rule.

When responding with either `FollowupAsync` or `RespondAsync` you can pass in an `ephemeral` property. When setting it to true it will respond ephemerally, false and it will respond non-ephemerally.

Let's use this in our list role command.

```cs
await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
```

Running the command now only shows the message to us!

![ephemeral command](images/ephemeral1.png)
