using System.Diagnostics;

namespace Discord;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class MessageApplication
{
    /// <summary>
    ///     Gets the snowflake ID of the application.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    ///     Gets the ID of the embed's image asset.
    /// </summary>
    public string CoverImage { get; internal set; }

    /// <summary>
    ///     Gets the application's description.
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    ///     Gets the ID of the application's icon.
    /// </summary>
    public string Icon { get; internal set; }

    /// <summary>
    ///     Gets the Url of the application's icon.
    /// </summary>
    public string IconUrl
        => $"https://cdn.discordapp.com/app-icons/{Id}/{Icon}";

    /// <summary>
    ///     Gets the name of the application.
    /// </summary>
    public string Name { get; internal set; }

    internal MessageApplication(ulong id, string coverImage, string description, string icon, string name)
    {
        Id = id;
        CoverImage = coverImage;
        Description = description;
        Icon = icon;
        Name = name;
    }

    private string DebuggerDisplay
        => $"{Name} ({Id}): {Description}";

    public override string ToString()
        => DebuggerDisplay;
}
