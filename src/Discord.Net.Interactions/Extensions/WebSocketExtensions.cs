using Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    internal static class WebSocketExtensions
    {
        /// <summary>
        ///     Get the name of the executed command and its parents in hierarchical order.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        ///     The name of the executed command and its parents in hierarchical order.
        /// </returns>
        public static IList<string> GetCommandKeywords(this IApplicationCommandInteractionData data)
        {
            var keywords = new List<string> { data.Name };

            var child = data.Options?.ElementAtOrDefault(0);

            while (child?.Type == ApplicationCommandOptionType.SubCommandGroup || child?.Type == ApplicationCommandOptionType.SubCommand)
            {
                keywords.Add(child.Name);
                child = child.Options?.ElementAtOrDefault(0);
            }

            return keywords;
        }

        /// <summary>
        ///     Get the name of the executed command and its parents in hierarchical order.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        ///     The name of the executed command and its parents in hierarchical order.
        /// </returns>
        public static IList<string> GetCommandKeywords(this IAutocompleteInteractionData data)
        {
            var keywords = new List<string> { data.CommandName };

            var group = data.Options?.FirstOrDefault(x => x.Type == ApplicationCommandOptionType.SubCommandGroup);
            if (group is not null)
                keywords.Add(group.Name);

            var subcommand = data.Options?.FirstOrDefault(x => x.Type == ApplicationCommandOptionType.SubCommand);
            if (subcommand is not null)
                keywords.Add(subcommand.Name);

            return keywords;
        }
    }
}
