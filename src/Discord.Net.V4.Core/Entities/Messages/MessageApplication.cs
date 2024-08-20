using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     Represents an application found within a <see cref="IMessage" />.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct MessageApplication : IModelConstructable<MessageApplication, IMessageApplicationModel>
{
    /// <summary>
    ///     The snowflake ID of the application.
    /// </summary>
    public readonly ulong Id;

    /// <summary>
    ///     The ID of the embed's image asset.
    /// </summary>
    public readonly string? CoverImage;

    /// <summary>
    ///     The application's description.
    /// </summary>
    public readonly string Description;

    /// <summary>
    ///     The ID of the application's icon.
    /// </summary>
    public readonly string Icon;

    /// <summary>
    ///     The name of the application.
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     The Url of the application's icon.
    /// </summary>
    public readonly string IconUrl
        => $"https://cdn.discordapp.com/app-icons/{Id}/{Icon}";

    internal MessageApplication(ulong id, string name, string description, string icon, string? coverImage = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
        CoverImage = coverImage;
    }

    private string DebuggerDisplay
        => $"{Name} ({Id}): {Description}";

    public static MessageApplication Construct(IDiscordClient client, IMessageApplicationModel model)
        => new(model.Id, model.Name, model.Description, model.Icon, model.CoverImage);

    public override string ToString()
        => DebuggerDisplay;
}
