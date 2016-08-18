using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Module
    {
        public TypeInfo Source { get; }
        public CommandService Service { get; }
        public string Name { get; }
        public string Prefix { get; }
        public string Summary { get; }
        public string Description { get; }
        public IEnumerable<Command> Commands { get; }
        internal object Instance { get; }

        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        internal Module(TypeInfo source, CommandService service, object instance, ModuleAttribute moduleAttr)
        {
            Source = source;
            Service = service;
            Name = source.Name;
            Prefix = moduleAttr.Prefix ?? "";
            Instance = instance;

            var summaryAttr = source.GetCustomAttribute<SummaryAttribute>();
            if (summaryAttr != null)
                Summary = summaryAttr.Text;

            var descriptionAttr = source.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
                Description = descriptionAttr.Text;

            List<Command> commands = new List<Command>();
            SearchClass(source, instance, commands, Prefix);
            Commands = commands;

            Preconditions = BuildPreconditions();
        }

        private void SearchClass(TypeInfo parentType, object instance, List<Command> commands, string groupPrefix)
        {
            if (groupPrefix != "")
                groupPrefix += " ";
            foreach (var method in parentType.DeclaredMethods)
            {
                var cmdAttr = method.GetCustomAttribute<CommandAttribute>();
                if (cmdAttr != null)
                    commands.Add(new Command(method, this, instance, cmdAttr, groupPrefix));
            }
            foreach (var type in parentType.DeclaredNestedTypes)
            {
                var groupAttrib = type.GetCustomAttribute<GroupAttribute>();
                if (groupAttrib != null)
                {
                    string nextGroupPrefix;
                    if (groupAttrib.Prefix != null)
                        nextGroupPrefix = groupPrefix + groupAttrib.Prefix ?? type.Name;
                    else
                        nextGroupPrefix = groupPrefix;
                    SearchClass(type, ReflectionUtils.CreateObject(type, Service), commands, nextGroupPrefix);
                }
            }
        }

        private IReadOnlyList<PreconditionAttribute> BuildPreconditions()
        {
            return Source.GetCustomAttributes<PreconditionAttribute>().ToImmutableArray();
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
