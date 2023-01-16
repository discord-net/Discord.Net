namespace Discord;

/// <summary>
///     Represents the layout type used to display posts in a forum channel.
/// </summary>
public enum ForumLayout
{
    /// <summary>
    /// A preferred forum layout hasn't been set by a server admin
    /// </summary>
    Default = 0,

    /// <summary>
    /// List View: display forum posts in a text-focused list
    /// </summary>
    List = 1,

    /// <summary>
    /// Gallery View: display forum posts in a media-focused gallery
    /// </summary>
    Grid = 2
}
