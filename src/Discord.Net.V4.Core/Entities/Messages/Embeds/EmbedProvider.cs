using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary> A provider field for an <see cref="Embed" />. </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedProvider : IEntityProperties<Models.Json.EmbedProvider>,
    IModelConstructable<EmbedProvider, IEmbedProviderModel>
{
    /// <summary>
    ///     The name of the provider.
    /// </summary>
    /// <returns>
    ///     A string representing the name of the provider.
    /// </returns>
    public readonly string? Name;

    /// <summary>
    ///     The URL of the provider.
    /// </summary>
    /// <returns>
    ///     A string representing the link to the provider.
    /// </returns>
    public readonly string? Url;

    internal EmbedProvider(string? name, string? url)
    {
        Name = name;
        Url = url;
    }

    private string DebuggerDisplay => $"{Name} ({Url})";

    /// <summary>
    ///     Gets the name of the provider.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Discord.EmbedProvider.Name" />.
    /// </returns>
    public override string? ToString() => Name;

    public static bool operator ==(EmbedProvider? left, EmbedProvider? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedProvider? left, EmbedProvider? right)
        => !(left == right);

    public Models.Json.EmbedProvider ToApiModel(Models.Json.EmbedProvider? existing = default)
    {
        existing ??= new Models.Json.EmbedProvider();

        existing.Name = Optional.FromNullable(Name);
        existing.Url = Optional.FromNullable(Url);

        return existing;
    }

    public static EmbedProvider Construct(IDiscordClient client, IEmbedProviderModel model) =>
        new(model.Name, model.Url);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedProvider" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedProvider" />, <see cref="Equals(EmbedProvider?)" /> will be called to
    ///     compare the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="EmbedProvider" /></param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedProvider embedProvider && Equals(embedProvider);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedProvider" /> is equal to the current <see cref="EmbedProvider" />
    /// </summary>
    /// <param name="embedProvider">
    ///     The <see cref="EmbedProvider" /> to compare with the current <see cref="EmbedProvider" />
    /// </param>
    /// <inheritdoc cref="Equals(object?)" />
    public bool Equals(EmbedProvider? embedProvider)
        => GetHashCode() == embedProvider?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Name, Url).GetHashCode();
}
