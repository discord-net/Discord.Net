using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     A field for an <see cref="Embed" />.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedField : IEntityProperties<Models.Json.EmbedField>,
    IConstructable<EmbedField, IEmbedFieldModel>
{
    /// <summary>
    ///     The name of the field.
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     The value of the field.
    /// </summary>
    public readonly string Value;

    /// <summary>
    ///     A value that indicates whether the field should be in-line with each other.
    /// </summary>
    public readonly bool? Inline;

    internal EmbedField(string name, string value, bool? inline)
    {
        Name = name;
        Value = value;
        Inline = inline;
    }

    private string DebuggerDisplay => $"{Name} ({Value}";

    /// <summary>
    ///     Gets the name of the field.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="EmbedField.Name" />.
    /// </returns>
    public override string ToString() => Name;

    public static bool operator ==(EmbedField? left, EmbedField? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedField? left, EmbedField? right)
        => !(left == right);

    public Models.Json.EmbedField ToApiModel(Models.Json.EmbedField? existing = default)
    {
        existing ??= new Models.Json.EmbedField {Name = Name, Value = Value};

        existing.Inline = Optional.FromNullable(Inline);

        return existing;
    }

    public static EmbedField Construct(IDiscordClient client, IEmbedFieldModel model) =>
        new(model.Name, model.Value, model.Inline);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedField" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedField" />, <see cref="Equals(EmbedField?)" /> will be called to compare
    ///     the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current object</param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedField embedField && Equals(embedField);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedField" /> is equal to the current <see cref="EmbedField" />
    /// </summary>
    /// <param name="embedField"></param>
    /// <inheritdoc cref="Object.Equals(object?)" />
    public bool Equals(EmbedField? embedField)
        => GetHashCode() == embedField?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Name, Value, Inline).GetHashCode();
}
