private static ComponentBuilderV2 BuildComponentUnsafe(Recipe recipe, Units units = Units.Si)
{
    StringBuilder ingredients = new($"## Ingredients{Environment.NewLine}");
    StringBuilder instructions = new($"## Instructions{Environment.NewLine}");
    ButtonBuilder servingsModalButton = new ButtonBuilder()
    .WithCustomId(RecipeServingsModal)
    .WithLabel("Set servings")
    .WithStyle(ButtonStyle.Primary);

    foreach (RecipeIngredient recipeIngredient in recipe.RecipeIngredients)
    {
        ingredients.Append($"`{recipeIngredient.Quantity} {recipeIngredient.Unit.ToSymbol()}` {recipeIngredient.Ingredient.Name} ");

        if (recipeIngredient.Optional)
            ingredients.AppendLine("[Optional]");
        else
            ingredients.Append(Environment.NewLine);
    }

    for (var i = 0; i < recipe.Instruction.Length; i++)
        instructions.AppendLine($"`{i + 1}.` {recipe.Instruction[i]}{Environment.NewLine}");

    return new ComponentBuilderV2()
    .WithTextDisplay($"# {recipe.Name}", RecipeNameDisplay)
    .WithTextDisplay($"-# {recipe.Servings} servings", RecipeServingsDisplay)
    .WithMediaGallery([
        "https://cdn.discordapp.com/attachments/964253122547552349/1336440069892083712/7Q3S.gif?ex=67a3d04e&is=67a27ece&hm=059c9d28466f43a50c4b450ca26fc01298a2080356421d8524384bf67ea8f3ab&"
    ])
    .WithActionRow([servingsModalButton])
    .WithTextDisplay(ingredients.ToString())
    .WithTextDisplay($"""
    ## Oven Settings
    Mode: `{recipe.OvenMode.ToHumanReadable()}`
    Temperature: `{recipe.Temperature.Convert(Unit.Temperature, Units.Si, units)} {units.ToSymbol()}`
    """)
    .WithActionRow([
        new SelectMenuBuilder(
            RecipeUnitInput,
            options:[
                new SelectMenuOptionBuilder(
                    "Metric",
                    "1",
                    isDefault: units == Units.Metric),
                    new SelectMenuOptionBuilder(
                        "Imperial",
                        "2",
                        isDefault: units == Units.Imperial),
                        new SelectMenuOptionBuilder(
                            "Kelvin",
                            "0",
                            isDefault: units == Units.Si)
            ],
            id: RecipeUnitSelectMenu
        )
    ])
    .WithTextDisplay(instructions.ToString());
}
