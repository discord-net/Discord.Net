---
uid: Guides.MessageComponents.GettingStarted
title: Getting Started with Components
---

# Message Components

Message components are a framework for adding interactive elements to a message your app or bot sends. They're accessible, customizable, and easy to use.

## What is a Component

Components are a new parameter you can use when sending messages with your bot. There are currently 2 different types of components you can use: Buttons and Select Menus.

## Creating components

Lets create a simple component that has a button. First thing we need is a way to trigger the message, this can be done via commands or simply a ready event. Lets make a command that triggers our button message.

```cs
[Command("spawner")]
public async Task Spawn()
{
    // Reply with some components
}
```

We now have our command, but we need to actually send the buttons with the command. To do that, lets look at the `ComponentBuilder` class:

| Name             | Description                                                                 |
| ---------------- | --------------------------------------------------------------------------- |
| `FromMessage`    | Creates a new builder from a message.                                       |
| `FromComponents` | Creates a new builder from the provided list of components.                 |
| `WithSelectMenu` | Adds a `SelectMenuBuilder` to the `ComponentBuilder` at the specific row.   |
| `WithButton`     | Adds a `ButtonBuilder` to the `ComponentBuilder` at the specific row.       |
| `Build`          | Builds this builder into a `MessageComponent` used to send your components. |

We see that we can use the `WithButton` function so lets do that. looking at its parameters it takes:

- `label` - The display text of the button.
- `customId` - The custom id of the button, this is whats sent by discord when your button is clicked.
- `style` - The discord defined style of the button.
- `emote` - An emote to be displayed with the button.
- `url` - The url of the button if its a link button.
- `disabled` - Whether or not the button is disabled.
- `row` - The row the button will occupy.

Since were just making a busic button, we dont have to specify anything else besides the label and custom id.

```cs
var builder = new ComponentBuilder()
    .WithButton("label", "custom-id");
```

Lets add this to our command:

```cs
[Command("spawner")]
public async Task Spawn()
{
    var builder = new ComponentBuilder()
        .WithButton("label", "custom-id");

    await ReplyAsync("Here is a button!", components: builder.Build());
}
```

![](images\image1.png)
