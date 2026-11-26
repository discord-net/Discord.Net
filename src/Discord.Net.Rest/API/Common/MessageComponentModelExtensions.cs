using System.Collections.Generic;

namespace Discord.API;

internal static class MessageComponentModelExtensions
{
    /// <summary>
    ///     Determines whether the given component models require the <see cref="MessageFlags.ComponentsV2"/> flag.
    /// </summary>
    internal static bool IsComponentsV2(this IEnumerable<IMessageComponent> components)
        => Preconditions.IsComponentsV2(components, static x => x.Type, static x => (x as ActionRowComponent)?.Components);
}
