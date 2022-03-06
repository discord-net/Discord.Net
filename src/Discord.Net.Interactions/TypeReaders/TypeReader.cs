using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base class for creating TypeConverters. <see cref="InteractionService"/> uses TypeConverters to interface with Slash Command parameters.
    /// </summary>
    public abstract class TypeReader : ITypeConverter<string>
    {
        /// <summary>
        ///     Will be used to search for alternative TypeReaders whenever the Command Service encounters an unknown parameter type.
        /// </summary>
        /// <param name="type">An object type.</param>
        /// <returns>
        ///     The boolean result.
        /// </returns>
        public abstract bool CanConvertTo(Type type);

        /// <summary>
        ///     Will be used to read the incoming payload before executing the method body.
        /// </summary>
        /// <param name="context">Command execution context.</param>
        /// <param name="option">Received option payload.</param>
        /// <param name="services">Service provider that will be used to initialize the command module.</param>
        /// <returns>The result of the read process.</returns>
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services);

        /// <summary>
        ///     Will be used to serialize objects into strings.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <returns>
        ///     A task representing the conversion process. The result of the task contains the conversion result.
        /// </returns>
        public virtual Task<string> SerializeAsync(object obj, IServiceProvider services) => Task.FromResult(obj.ToString());
    }

    /// <inheritdoc/>
    public abstract class TypeReader<T> : TypeReader
    {
        /// <inheritdoc/>
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
