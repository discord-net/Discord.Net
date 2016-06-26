using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Module
    {
        public CommandService Service { get; }
        public string Name { get; }
        public IEnumerable<Command> Commands { get; }

        internal Module(CommandService service, object instance, TypeInfo typeInfo)
        {
            Service = service;
            Name = typeInfo.Name;

            List<Command> commands = new List<Command>();
            SearchClass(instance, commands, typeInfo);
            Commands = commands;
        }

        private void SearchClass(object instance, List<Command> commands, TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.DeclaredMethods)
            {
                var cmdAttr = method.GetCustomAttribute<CommandAttribute>();
                if (cmdAttr != null)
                    commands.Add(new Command(this, instance, cmdAttr, method));
            }
            foreach (var type in typeInfo.DeclaredNestedTypes)
            {
                if (type.GetCustomAttribute<GroupAttribute>() != null)
                    SearchClass(ReflectionUtils.CreateObject(type), commands, type);
            }
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
