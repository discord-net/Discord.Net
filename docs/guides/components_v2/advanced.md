---
uid: Guides.ComponentsV2.Advanced
title: Create Components V2
---

# Component types

As denoted in the **intro** page, This framework supports a lot of component types. This example offers some more insight into what your options are. Below is a component with `TextDisplay`, `MediaGallery` and `ActionRow` (with `Buttons` or `SelectMenu`).

![](images/interaction-response.png)

## Code

Some code will not be included here as it is not relevant to this framework. If you want to see the full code, it is [here](https://github.com/Adrigorithm/Adribot/).

The main component container generation method:
[!code-csharp[Logging Sample](samples/component.cs)]

## Interactions

The button triggers the following modal

![](images/modal.png)

```cs
private static ModalBuilder CreateServingsModal(short servings)
{
    TextInputBuilder? textInput = new TextInputBuilder()
        .WithCustomId(RecipeServingsInput)
        .WithLabel("Servings")
        .WithValue(servings.ToString())
        .WithMinLength(1)
        .WithMaxLength(3)
        .WithStyle(TextInputStyle.Short);

    return new ModalBuilder()
        .WithCustomId(RecipeServingsButton)
        .WithTitle("Set Servings")
        .AddTextInput(textInput);
}
```

Other interactions used by this message:

```cs
private async Task ClientOnInteractionCreatedAsync(SocketInteraction arg)
{
    switch (arg)
    {
        case SocketMessageComponent component:
            switch (component.Data.CustomId)
            {
                // SET SERVINGS BUTTON CLICKED
                case RecipeServingsModal:
                    var servings = short.Parse(component.Message.Components.FindComponentById<TextDisplayComponent>(RecipeServingsDisplay).Content.Split(' ')[1]);

                    await component.RespondWithModalAsync(CreateServingsModal(servings).Build());

                    break;

                // ITEM IN COMBOXBOX CHANGED
                case RecipeUnitInput:
                    SelectMenuComponent selectedItem = component.Message.Components.FindComponentById<SelectMenuComponent>(RecipeUnitSelectMenu);
                    var unitValue = short.Parse(component.Data.Values.First());
                    var recipeName = component.Message.Components.FindComponentById<TextDisplayComponent>(RecipeNameDisplay).Content[2..];
                    Recipe recipe = _recipes.First(r => r.Name == recipeName);
                    Recipe recipe0 = recipe.Clone();
                    var unit = (Units)Enum.ToObject(typeof(Units), unitValue);

                    ComponentBuilderV2 newComponentContainer = BuildComponentUnsafe(recipe0, unit);

                    await component.UpdateAsync(m => m.Components = newComponentContainer.Build());

                    break;
                default:
                    // Ununsed here
            }

            break;

        // MODAL SUBMIT
        case SocketModal modal:
            if (modal.Data.CustomId == RecipeServingsButton)
            {
                var success = short.TryParse(modal.Data.Components.First(c => c.CustomId == RecipeServingsInput).Value, out var servings);

                if (!success || servings <= 0)
                    break;

                Recipe recipe = _recipes.First(r => r.Name == modal.Message.Components.FindComponentById<TextDisplayComponent>(RecipeNameDisplay).Content[2..]);
                Recipe? recipe0 = recipe.Clone();

                recipe0.ChangeServings(servings, true);

                ComponentBuilderV2 newComponentContainer = BuildComponentUnsafe(recipe0);

                await modal.UpdateAsync(m => m.Components = newComponentContainer.Build());
            }

            break;
        default:
            return;
    }
}
```
After the **SET SERVINGS** modal is submitted (or the COMBOXBOX is changed) the UI is updated:

![](images/updated-ingredients.png)

![](images/updated-oven.png)

## Troubleshooting

If you run into any trouble (appliction not responding when sending components), a debugger is (like usually) a useful tool to have at your disposal. More specifically within this context: dnet will do some checks before sending the component configuration to discord, so on building the component array, you can check for errors thrown. If this does not help, setting a breakpoint on the line that sends your component to discord (ModifyAsync/RespondAsync/...) and stepping to the next line (within 3 seconds of triggering it) may yield a more specific error returned by Discord itself.
