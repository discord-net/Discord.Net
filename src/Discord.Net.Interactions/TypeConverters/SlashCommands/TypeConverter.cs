using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base class for creating TypeConverters. <see cref="InteractionService"/> uses TypeConverters to interface with Slash Command parameters.
    /// </summary>
    public abstract class TypeConverter : ITypeConverter<IApplicationCommandInteractionDataOption>
    {
        /// <summary>
        ///     Will be used to search for alternative TypeConverters whenever the Command Service encounters an unknown parameter type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool CanConvertTo(Type type);

        /// <summary>
        ///     Will be used to get the Application Command Option type.
        /// </summary>
        /// <returns>The option type.</returns>
        public abstract ApplicationCommandOptionType GetDiscordType();

        /// <summary>
        ///     Will be used to read the incoming payload before executing the method body.
        /// </summary>
        /// <param name="context">Command execution context.</param>
        /// <param name="option">Received option payload.</param>
        /// <param name="services">Service provider that will be used to initialize the command module.</param>
        /// <returns>The result of the read process.</returns>
        public abstract Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services);

        /// <summary>
        ///     Will be used to manipulate the outgoing command option, before the command gets registered to Discord.
        /// </summary>
        public virtual void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter) { }
    }

    /// <inheritdoc/>
    public abstract class TypeConverter<T> : TypeConverter
    {
        /// <inheritdoc/>
        public sealed override bool CanConvertTo(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
