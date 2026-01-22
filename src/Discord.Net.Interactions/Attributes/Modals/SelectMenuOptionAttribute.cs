using System;

namespace Discord.Interactions;

/// <summary>
///     Adds additional metadata to enum fields that are used for select-menus.
/// </summary>
/// <remarks>
///     To manually add select menu options to modal components, use <see cref="ModalSelectMenuOptionAttribute"/> instead.
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SelectMenuOptionAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the desription of the option.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the option is selected by default.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    ///     Gets or sets the emote of the option.
    /// </summary>
    /// <remarks>
    ///     Can be either an <see cref="Emoji"/> or an <see cref="Discord.Emote"/>
    /// </remarks>
    public string Emote { get; set; }
}
