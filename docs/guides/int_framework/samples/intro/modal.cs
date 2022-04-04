// Registers a command that will respond with a modal.
[SlashCommand("food", "Tell us about your favorite food.")]
public async Task Command()
    => await Context.Interaction.RespondWithModalAsync<FoodModal>("food_menu");

// Defines the modal that will be sent.
public class FoodModal : IModal
{
    public string Title => "Fav Food";
    // Strings with the ModalTextInput attribute will automatically become components.
    [InputLabel("What??")]
    [ModalTextInput("food_name", placeholder: "Pizza", maxLength: 20)]
    public string Food { get; set; }

    // Additional paremeters can be specified to further customize the input.
    [InputLabel("Why??")]
    [ModalTextInput("food_reason", TextInputStyle.Paragraph, "Kuz it's tasty", maxLength: 500)]
    public string Reason { get; set; }
}

// Responds to the modal.
[ModalInteraction("food_menu")]
public async Task ModalResponse(FoodModal modal)
{
    // Build the message to send.
    string message = "hey @everyone, I just learned " +
        $"{Context.User.Mention}'s favorite food is " +
        $"{modal.Food} because {modal.Reason}.";

    // Specify the AllowedMentions so we don't actually ping everyone.
    AllowedMentions mentions = new();
    mentions.AllowedTypes = AllowedMentionTypes.Users;

    // Respond to the modal.
    await RespondAsync(message, allowedMentions: mentions, ephemeral: true);
}