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
