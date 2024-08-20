using Discord.Models;
using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Represents a message component on a message.
/// </summary>
public interface IMessageComponent : IEntityProperties<MessageComponent>,
    IModelConstructable<IMessageComponent, IMessageComponentModel>
{
    /// <summary>
    ///     Gets the <see cref="ComponentType" /> of this Message Component.
    /// </summary>
    ComponentType Type { get; }

    /// <summary>
    ///     Gets the custom id of the component if possible; otherwise <see langword="null" />.
    /// </summary>
    string? CustomId { get; }

    static IMessageComponent IModelConstructable<IMessageComponent, IMessageComponentModel, IDiscordClient>.Construct(
        IDiscordClient client, IMessageComponentModel model)
        => Construct(client, model);

    new static IMessageComponent Construct(IDiscordClient client, IMessageComponentModel model)
    {
        switch (model)
        {
            case IActionRowModel actionRowModel:
                return ActionRowComponent.Construct(client, actionRowModel);
            case IButtonComponentModel buttonComponentModel:
                return ButtonComponent.Construct(client, buttonComponentModel);
            case ISelectMenuComponentModel selectMenuComponentModel:
                return SelectMenuComponent.Construct(client, selectMenuComponentModel);
            case ITextInputComponentModel textInputComponentModel:
                return TextInputComponent.Construct(client, textInputComponentModel);
            default:
                throw new ArgumentOutOfRangeException(nameof(model));
        }
    }
}
