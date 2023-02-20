using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base class for creating Component TypeConverters. <see cref="InteractionService"/> uses TypeConverters to interface with Slash Command parameters.
    /// </summary>
    public abstract class ComponentTypeConverter : ITypeConverter<IComponentInteractionData>
    {
        /// <summary>
        ///     Will be used to search for alternative TypeConverters whenever the Command Service encounters an unknown parameter type.
        /// </summary>
        /// <param name="type">An object type.</param>
        /// <returns>
        ///     The boolean  result.
        /// </returns>
        public abstract bool CanConvertTo(Type type);

        /// <summary>
        ///     Will be used to read the incoming payload before executing the method body.
        /// </summary>
        /// <param name="context">Command execution context.</param>
        /// <param name="option">Received option payload.</param>
        /// <param name="services">Service provider that will be used to initialize the command module.</param>
        /// <returns>
        ///     The result of the read process.
        /// </returns>
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services);
    }

    /// <inheritdoc/>
    public abstract class ComponentTypeConverter<T> : ComponentTypeConverter
    {
        /// <inheritdoc/>
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
