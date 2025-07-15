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
