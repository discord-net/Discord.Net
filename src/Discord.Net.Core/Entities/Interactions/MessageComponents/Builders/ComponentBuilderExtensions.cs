using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public static class ComponentBuilderExtensions
{
    /// <summary>
    ///     Sets the custom id for the component.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public static BuilderT WithId<BuilderT>(this BuilderT builder, int? id)
        where BuilderT : IMessageComponentBuilder
    {
        builder.Id = id;
        return builder;
    }
    
    /// <summary>
    ///     Converts a collection of <see cref="IMessageComponent"/> to a <see cref="ComponentBuilderV2"/>.
    /// </summary>
    public static ComponentBuilderV2 ToBuilder(this IEnumerable<IMessageComponent> components)
        => new(components);
}
