using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Interactions.Utilities;

internal record EnumSelectMenuOption(
    SelectMenuOption MenuOption,
    Predicate<IDiscordInteraction> Predicate,
    object Value);

internal class EnumUtils
{
    public static IEnumerable<EnumSelectMenuOption> BuildSelectMenuOptions(Type enumType)
    {
        if(!enumType.IsEnum)
            throw new ArgumentException($"Type {enumType} is not an enum");

        var names = Enum.GetNames(enumType);
        var members = names.SelectMany(x => enumType.GetMember(x));

        foreach (var member in members)
        {
            var selectMenuOptionAttr = member.GetCustomAttribute<SelectMenuOptionAttribute>();

            Emoji emoji = null;
            Emote emote = null;

            if (!string.IsNullOrEmpty(selectMenuOptionAttr?.Emote) && !(Emote.TryParse(selectMenuOptionAttr.Emote, out emote) || Emoji.TryParse(selectMenuOptionAttr.Emote, out emoji)))
                throw new ArgumentException($"Unable to parse {selectMenuOptionAttr.Emote} of {member.DeclaringType.Name}.{member.Name} into an {nameof(Emote)} or an {nameof(Emoji)}");

            var hideAttr = member.GetCustomAttribute<HideAttribute>();
            Predicate<IDiscordInteraction> predicate = hideAttr != null ? hideAttr.Predicate : null;

            var value = Enum.Parse(enumType, member.Name);
            var optionBuilder = new SelectMenuOptionBuilder(member.GetCustomAttribute<ChoiceDisplayAttribute>()?.Name ?? member.Name,
                member.Name, selectMenuOptionAttr?.Description, emote != null ? emote : emoji, selectMenuOptionAttr?.IsDefault);

            yield return new EnumSelectMenuOption(optionBuilder.Build(), predicate, value);
        }
    }
}
