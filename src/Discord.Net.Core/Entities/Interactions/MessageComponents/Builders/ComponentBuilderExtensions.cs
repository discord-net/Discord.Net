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
}
