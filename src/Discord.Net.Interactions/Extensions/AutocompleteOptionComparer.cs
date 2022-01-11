using System.Collections.Generic;

namespace Discord.Interactions
{
    internal class AutocompleteOptionComparer : IComparer<ApplicationCommandOptionType>
    {
        public int Compare(ApplicationCommandOptionType x, ApplicationCommandOptionType y)
        {
            if (x == ApplicationCommandOptionType.SubCommandGroup)
            {
                if (y == ApplicationCommandOptionType.SubCommandGroup)
                    return 0;
                else
                    return 1;
            }
            else if (x == ApplicationCommandOptionType.SubCommand)
            {
                if (y == ApplicationCommandOptionType.SubCommandGroup)
                    return -1;
                else if (y == ApplicationCommandOptionType.SubCommand)
                    return 0;
                else
                    return 1;
            }
            else
            {
                if (y == ApplicationCommandOptionType.SubCommand || y == ApplicationCommandOptionType.SubCommandGroup)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
