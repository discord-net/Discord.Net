private async Task<ComponentBuilderV2> BuildComponentsUnsafeAsync()
{
    if (!_recipes.Any()) // _recipes is simply a list of recipe objects
    {
        return new ComponentBuilderV2()
        .WithTextDisplay(
            """
            # No recipes found
            You should consider adding some.
            """);
    }

    var builder = new ComponentBuilderV2();
    Emote? emote = await _clientProvider.Client.GetApplicationEmoteAsync(1393996479357517925);

    foreach (Recipe recipe in _recipes)
    {
        var buttonBuilder = new ButtonBuilder("Look inside", $"{RecipesLookInsideButton}-{recipe.RecipeId}"); // RecipesLookInsideButton is a constant string

        if (emote is not null)
            buttonBuilder.WithEmote(emote);

        builder
        .WithTextDisplay($"# {recipe.Name}")
        .WithMediaGallery(["https://cdn.discordapp.com/attachments/964253122547552349/1336440069892083712/7Q3S.gif?ex=67a3d04e&is=67a27ece&hm=059c9d28466f43a50c4b450ca26fc01298a2080356421d8524384bf67ea8f3ab&"])
        .WithActionRow([
            buttonBuilder
        ]);
    }

    return builder;
}
