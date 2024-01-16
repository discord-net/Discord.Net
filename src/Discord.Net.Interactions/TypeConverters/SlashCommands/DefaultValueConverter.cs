using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class DefaultValueConverter<T> : TypeConverter<T> where T : IConvertible
    {
        public override ApplicationCommandOptionType GetDiscordType()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return ApplicationCommandOptionType.Boolean;

                case TypeCode.DateTime:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Single:
                    return ApplicationCommandOptionType.String;

                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return ApplicationCommandOptionType.Integer;

                case TypeCode.Decimal:
                case TypeCode.Double:
                    return ApplicationCommandOptionType.Number;

                case TypeCode.DBNull:
                default:
                    throw new InvalidOperationException($"Parameter Type {typeof(T).FullName} is not supported by Discord.");
            }
        }
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            object value;

            if (option.Value is Optional<object> optional)
                value = optional.IsSpecified ? optional.Value : default(T);
            else
                value = option.Value;

            try
            {
                var converted = Convert.ChangeType(value, typeof(T));
                return Task.FromResult(TypeConverterResult.FromSuccess(converted));
            }
            catch (InvalidCastException castEx)
            {
                return Task.FromResult(TypeConverterResult.FromError(castEx));
            }
        }
    }
}
