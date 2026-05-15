using System;

namespace Discord.Interactions;

/// <summary>
///     Adds additional metadata to enum fields that are used for select-menus, checkbox groups, and radio groups.
/// </summary>
/// <remarks>
///     To manually add select menu, checkbox group or radio group options to modal components,
///     instead use <see cref="ModalSelectMenuOptionAttribute"/>, <see cref="ModalCheckboxGroupOptionAttribute"/>,
///     and <see cref="ModalRadioGroupOptionAttribute"/> respectively.
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumOptionAttribute : Attribute
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
    ///     Can be either an <see cref="Emoji"/> or an <see cref="Discord.Emote"/>.
    ///     <br/>
    ///     <b>Only applicable for select menus.</b>
    /// </remarks>
    public string Emote { get; set; }
}
