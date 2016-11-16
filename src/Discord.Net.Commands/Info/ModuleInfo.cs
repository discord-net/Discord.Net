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
        public IEnumerable<CommandInfo> Commands { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        internal ModuleInfo(ModuleBuilder builder, CommandService service)
        {
            Service = service;

            Name = builder.Name;
            Summary = builder.Summary;
            Remarks = builder.Remarks;

            Aliases = BuildAliases(builder).ToImmutableArray();
            Commands = builder.Commands.Select(x => x.Build(this, service));
            Preconditions = BuildPreconditions(builder).ToImmutableArray();
        }

        private static IEnumerable<string> BuildAliases(ModuleBuilder builder)
        {
            IEnumerable<string> result = null;

            Stack<ModuleBuilder> builderStack = new Stack<ModuleBuilder>();
            builderStack.Push(builder);

            ModuleBuilder parent = builder.ParentModule;
            while (parent != null)
            {
                builderStack.Push(parent);
                parent = parent.ParentModule;
            }

            while (builderStack.Count() > 0)
            {
                ModuleBuilder level = builderStack.Pop(); // get the topmost builder
                if (result == null)
                    result = level.Aliases.ToList(); // create a shallow copy so we don't overwrite the builder unexpectedly
                else if (result.Count() > level.Aliases.Count)
                    result = result.Permutate(level.Aliases, (first, second) => first + " " + second);
                else
                    result = level.Aliases.Permutate(result, (second, first) => first + " " + second);
            }

            return result;
        }

        private static List<PreconditionAttribute> BuildPreconditions(ModuleBuilder builder)
        {
            var result = new List<PreconditionAttribute>();


            ModuleBuilder parent = builder;
            while (parent.ParentModule != null)
            {
                result.AddRange(parent.Preconditions);
                parent = parent.ParentModule;
            }

            return result;
        }
    }
}