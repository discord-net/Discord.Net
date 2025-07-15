[SlashCommand("recipes", "Gets all recipes")]
public async Task GetRecipesAsync()
{
    MessageComponent? embed = (await recipeService.GetRecipesComponentAsync())?.Build();

    if (embed is null)
    {
        await RespondAsync($"No recipes found.", ephemeral: true);

        return;
    }

    await RespondAsync(components: embed);
}
