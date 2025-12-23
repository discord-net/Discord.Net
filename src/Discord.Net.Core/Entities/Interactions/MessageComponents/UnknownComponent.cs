namespace Discord;

/// <summary>
///     Represents an unknown message component type that Discord has sent but is not yet supported by the library.
/// </summary>
public class UnknownComponent : IMessageComponent
{
    /// <summary>
    ///     Gets the raw component type value from Discord.
    /// </summary>
    public int RawType { get; }

    /// <inheritdoc/>
    public ComponentType Type => (ComponentType)RawType;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the raw JSON data of this component.
    /// </summary>
    public string RawJson { get; }

    internal UnknownComponent(int rawType, string rawJson, int? id = null)
    {
        RawType = rawType;
        RawJson = rawJson;
        Id = id;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder()
        => throw new System.NotSupportedException("Unknown components cannot be converted to builders.");
}
