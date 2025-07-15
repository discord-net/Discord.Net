private async Task ClientOnInteractionCreatedAsync(SocketInteraction arg)
{
    switch (arg)
    {
        case SocketMessageComponent component:
            switch (component.Data.CustomId)
            {
                // Non dynamic cases ...

                default:
                    var customId = component.Data.CustomId;
                    var lastPartStartIndex = customId.LastIndexOf('-');

                    if (lastPartStartIndex == -1)
                        return;

                if (customId[..lastPartStartIndex] == RecipesLookInsideButton) // "recipes-show-me-button"
                    await component.UpdateAsync(m => m.Components = BuildComponentUnsafe(_recipes.First(r => r.RecipeId == int.Parse(customId[(lastPartStartIndex + 1)..]))).Build()); // _recipes is a list of Recipe objects ; int.Parse({recipe.RecipeId}) (in this case it is 1)

                    break;
            }

            break;
                case SocketModal modal:
                    // Interaction came from a modal

                    break;
                default:
                    return;
    }
}
