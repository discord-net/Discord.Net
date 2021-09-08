# Responding to button clicks

Responding to buttons is pretty simple, there are a couple ways of doing it and we can cover both.

### Method 1: Hooking the InteractionCreated Event

We can hook the `InteractionCreated` event since button clicks are a form of interactions:

```cs
client.IntreactionCreated += MyInteractionHandler;
```

Now, lets write our handler.

```cs
public async Task MyInteractionHandler(SocketInteraction arg)
{
    // first we check the type of the interaction, this can be done with a switch statement
    switch(arg)
    {
        case SocketMessageComponent component:
            // we now have a variable defined as 'component' which contains our component data, lets pass it to a different handler.

        break;
    }
}

public async Task MyButtonHandler(SocketMessageComponent component)
{
    // We can now check for our custom id
    switch(component.Data.CustomId)
    {
        // Since we set our buttons custom id as 'custom-id', we can check for it like this:
        case "custom-id":
            // Lets respond by sending a message saying they clicked the button
            await component.RespondAsync($"{component.User.Mention} has clicked the button!");
        break;
    }
}
```

Running it and clicking the button:

![](Images/image2.png)

### Method 2: Hooking the ButtonExecuted Event

This method skips the first switch statement because the `ButtonExecuted` event is only fired when a button is clicked, meaning we dont have to check the type of the interaction.

```cs
client.ButtonExecuted += MyButtonHandler;
```

The rest of the code is the same and produces the same result.
