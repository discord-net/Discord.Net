using System;
using System.Collections.Generic;

namespace Discord.Interactions
{
    internal static class CommandHierarchy
    {
        public const char EscapeChar = '$';

        public static IList<string> GetModulePath(this ModuleInfo moduleInfo)
        {
            var result = new List<string>();

            var current = moduleInfo;
            while (current is not null)
            {
                if (current.IsSlashGroup)
                    result.Insert(0, current.SlashGroupName);

                current = current.Parent;
            }

            return result;
        }

        public static IList<string> GetCommandPath(this ICommandInfo commandInfo)
        {
            if (commandInfo.IgnoreGroupNames)
                return new List<string> { commandInfo.Name };

            var path = commandInfo.Module.GetModulePath();
            path.Add(commandInfo.Name);
            return path;
        }

        public static IList<string> GetParameterPath(this IParameterInfo parameterInfo)
        {
            var path = parameterInfo.Command.GetCommandPath();
            path.Add(parameterInfo.Name);
            return path;
        }

        public static IList<string> GetChoicePath(this IParameterInfo parameterInfo, ParameterChoice choice)
        {
            var path = parameterInfo.GetParameterPath();
            path.Add(choice.Name);
            return path;
        }

        public static IList<string> GetTypePath(Type type) =>
            new List<string> { EscapeChar + type.FullName };
    }
}
