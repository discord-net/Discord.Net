using Discord.Models;
using Discord.Models.Json;
using System.Collections.Immutable;

namespace Discord;

public sealed class ActionRowComponent : IMessageComponent, IModelConstructable<ActionRowComponent, IActionRowModel>
{
    internal ActionRowComponent(IEnumerable<IMessageComponent> components)
    {
        Components = components.ToImmutableArray();
    }

    /// <summary>
    ///     Gets the child components in this row.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    public static ActionRowComponent Construct(IDiscordClient client, IActionRowModel model)
        => new(model.Components.Select(x => IMessageComponent.Construct(client, x)));

    /// <inheritdoc />
    public ComponentType Type
        => ComponentType.ActionRow;

    /// <inheritdoc />
    /// <remarks>
    ///     This property is always <see langword="null" /> for <see cref="ActionRowComponent" />.
    /// </remarks>
    public string? CustomId
        => null;

    public MessageComponent ToApiModel(MessageComponent? existing = default) =>
        existing ?? new ActionRow {Type = (uint)Type, Components = Components.Select(x => x.ToApiModel()).ToArray()};
}
