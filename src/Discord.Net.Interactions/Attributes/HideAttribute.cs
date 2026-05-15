using System;

namespace Discord.Interactions;

/// <summary>
///     Enum values tagged with this attribute will not be displayed as a parameter choice
/// </summary>
/// <remarks>
///     This attribute must be used along with the default <see cref="EnumConverter{T}"/> and <see cref="DefaultEntityTypeConverter{T}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class HideAttribute : Attribute
{
    /// <summary>
    ///     Can be optionally implemented by inherited types to conditionally hide an enum value.
    /// </summary>
    /// <remarks>
    ///     Only runs on prior to modal construction. For slash command parameters, this method is ignored.
    /// </remarks>
    /// <param name="interaction">Interaction that <see cref="IDiscordInteractionExtentions.RespondWithModalAsync{T}(IDiscordInteraction, string, T, RequestOptions, Action{ModalBuilder})"/> is called on.</param>
    /// <returns>
    ///     <see langword="true"/> if the attribute should be active and hide the value.
    /// </returns>
    public virtual bool Predicate(IDiscordInteraction interaction) => true;
}
