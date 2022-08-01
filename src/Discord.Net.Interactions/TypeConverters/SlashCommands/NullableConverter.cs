using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class NullableConverter<T> : TypeConverter<T>
    {
        private readonly TypeConverter _typeConverter;

        public NullableConverter(InteractionService interactionService, IServiceProvider services)
        {
            var nullableType = typeof(T);
            var type = Nullable.GetUnderlyingType(nullableType);

            if (type is null)
                throw new ArgumentException($"No type {nameof(TypeConverter)} is defined for this {nullableType.FullName}", nameof(type));

            _typeConverter = interactionService.GetTypeConverter(type, services);
        }

        public override ApplicationCommandOptionType GetDiscordType()
            => _typeConverter.GetDiscordType();

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
            => _typeConverter.ReadAsync(context, option, services);

        public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter)
            => _typeConverter.Write(properties, parameter);
    }
}
