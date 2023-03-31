using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class EnumConverter<T> : TypeConverter<T> where T : struct, Enum
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (Enum.TryParse<T>((string)option.Value, out var result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {nameof(T)}"));
        }

        public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameterInfo)
        {
            var names = Enum.GetNames(typeof(T));
            var members = names.SelectMany(x => typeof(T).GetMember(x)).Where(x => !x.IsDefined(typeof(HideAttribute), true));

            if (members.Count() <= 25)
            {
                var choices = new List<ApplicationCommandOptionChoiceProperties>();

                foreach (var member in members)
                {
                    var displayValue = member.GetCustomAttribute<ChoiceDisplayAttribute>()?.Name ?? member.Name;
                    choices.Add(new ApplicationCommandOptionChoiceProperties
                    {
                        Name = displayValue,
                        Value = member.Name
                    });
                }
                properties.Choices = choices;
            }
        }
    }

    /// <summary>
    ///     Enum values tagged with this attribute will not be displayed as a parameter choice
    /// </summary>
    /// <remarks>
    ///     This attribute must be used along with the default <see cref="EnumConverter{T}"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class HideAttribute : Attribute { }
}
