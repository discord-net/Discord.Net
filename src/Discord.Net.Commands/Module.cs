using System.Collections.Generic;
using System.Reflection;

namespace Discord.Commands
{
    public class Module
    {
        public string Name { get; }
        public IEnumerable<Command> Commands { get; }

        internal Module(object parent, TypeInfo typeInfo)
        {
            List<Command> commands = new List<Command>();
            SearchClass(parent, commands, typeInfo);
            Commands = commands;
        }

        private void SearchClass(object parent, List<Command> commands, TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.DeclaredMethods)
            {
                var cmdAttr = method.GetCustomAttribute<CommandAttribute>();
                if (cmdAttr != null)
                    commands.Add(new Command(cmdAttr, method));
            }
            foreach (var type in typeInfo.DeclaredNestedTypes)
            {
                if (type.GetCustomAttribute<GroupAttribute>() != null)
                    SearchClass(ReflectionUtils.CreateObject(type), commands, type);
            }
        }
    }
}
