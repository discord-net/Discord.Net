using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    public class ModuleInfo
    {
        public CommandService Service { get; }
        public string Name { get; }
        public string Summary { get; }
        public string Remarks { get; }

        public IReadOnlyList<string> Aliases { get; }
        public IReadOnlyList<CommandInfo> Commands { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }
        public IReadOnlyList<ModuleInfo> Submodules { get; }
        public ModuleInfo Parent { get; }
        public bool IsSubmodule => Parent != null;

        internal ModuleInfo(ModuleBuilder builder, CommandService service, ModuleInfo parent = null)
        {
            Service = service;

            Name = builder.Name;
            Summary = builder.Summary;
            Remarks = builder.Remarks;
            Parent = parent;

            Aliases = BuildAliases(builder, service).ToImmutableArray();
            Commands = builder.Commands.Select(x => x.Build(this, service)).ToImmutableArray();
            Preconditions = BuildPreconditions(builder).ToImmutableArray();

            Submodules = BuildSubmodules(builder, service).ToImmutableArray();
        }

        private static IEnumerable<string> BuildAliases(ModuleBuilder builder, CommandService service)
        {
            IEnumerable<string> result = null;

            Stack<ModuleBuilder> builderStack = new Stack<ModuleBuilder>();
            builderStack.Push(builder);

            ModuleBuilder parent = builder.Parent;
            while (parent != null)
            {
                builderStack.Push(parent);
                parent = parent.Parent;
            }

            while (builderStack.Count() > 0)
            {
                ModuleBuilder level = builderStack.Pop(); //get the topmost builder
                if (result == null)
                {
                    if (level.Aliases.Count > 0)
                        result = level.Aliases.ToList(); //create a shallow copy so we don't overwrite the builder unexpectedly
                }
                else if (result.Count() > level.Aliases.Count)
                    result = result.Permutate(level.Aliases, (first, second) => first + service._splitCharacter + second);
                else
                    result = level.Aliases.Permutate(result, (second, first) => first + service._splitCharacter + second);
            }

            if (result == null) //there were no aliases; default to an empty list
                result = new List<string>();

            return result;
        }

        private List<ModuleInfo> BuildSubmodules(ModuleBuilder parent, CommandService service)
        {
            var result = new List<ModuleInfo>();

            foreach (var submodule in parent.Modules)
            {
                result.Add(submodule.Build(service, this));
            }

            return result;
        }

        private static List<PreconditionAttribute> BuildPreconditions(ModuleBuilder builder)
        {
            var result = new List<PreconditionAttribute>();

            ModuleBuilder parent = builder;
            while (parent != null)
            {
                result.AddRange(parent.Preconditions);
                parent = parent.Parent;
            }

            return result;
        }
    }
}
