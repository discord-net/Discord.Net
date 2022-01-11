using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base class for creating <see cref="TypeReader"/>s. <see cref="InteractionService"/> uses <see cref="TypeReader"/>s to parse string values into entities.
    /// </summary>
    /// <remarks>
    ///     <see cref="TypeReader"/>s are mainly used to parse message component values. For interfacing with Slash Command parameters use <see cref="TypeConverter"/>s instead.
    /// </remarks>
    public abstract class TypeReader : ITypeHandler
    {
        /// <summary>
        ///     Will be used to search for alternative TypeReaders whenever the Command Service encounters an unknown parameter type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool CanConvertTo(Type type);

        /// <summary>
        ///     Will be used to read the incoming payload before executing the method body.
        /// </summary>
        /// <param name="context">Command exexution context.</param>
        /// <param name="input">Raw string input value.</param>
        /// <param name="services">Service provider that will be used to initialize the command module.</param>
        /// <returns>The result of the read process.</returns>
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services);

        /// <summary>
        ///     Will be used to manipulate the outgoing command option, before the command gets registered to Discord.
        /// </summary>
        public virtual string Serialize(object value) => null;
    }

    /// <inheritdoc/>
    public abstract class TypeReader<T> : TypeReader
    {
        /// <inheritdoc/>
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
