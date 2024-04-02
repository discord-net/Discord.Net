---
uid: Guides.MessageComponents.Buttons
title: Buttons in Depth
---

# Buttons in depth

There are many changes you can make to buttons, lets take a look at the parameters in the `WithButton` function.

| Name | Type | Description |
|----------|---------------|----------------------------------------------------------------|
| label | `string` | The label text for the button. |
| customId | `string` | The custom id of the button. |
| style | `ButtonStyle` | The style of the button. |
| emote | `IEmote` | A IEmote to be used with this button. |
| url | `string` | A URL to be used only if the `ButtonStyle` is a Link. |
| disabled | `bool` | Whether or not the button is disabled. |
| row | `int` | The row to place the button if it has enough room, otherwise 0 |

### Label

This is the front facing text that the user sees. The maximum length is 80 characters.

### CustomId

This is the property sent to you by discord when a button is clicked. It is not required for link buttons as they do not emit an event. The maximum length is 100 characters.

### Style

Styling your buttons are important for indicating different actions:

![](images/image3.png)

You can do this by using the `ButtonStyle` which has all the styles defined.

### Emote

You can specify an `IEmote` when creating buttons to add them to your button. They have the same restrictions as putting guild based emotes in messages.

### Url

If you use the link style with your button you can specify a url. When this button is clicked the user is taken to that url.

### Disabled

You can specify if your button is disabled, meaning users won't be able to click on it.
