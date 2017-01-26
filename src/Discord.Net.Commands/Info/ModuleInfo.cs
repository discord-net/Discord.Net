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
            var result = builder.Aliases.ToList();
            var builderQueue = new Queue<ModuleBuilder>();

            var parent = builder;
            while ((parent = parent.Parent) != null)
                builderQueue.Enqueue(parent);

            while (builderQueue.Count > 0)
            {
                var level = builderQueue.Dequeue();
                // permute in reverse because we want to *prefix* our aliases
                result = level.Aliases.Permutate(result, (first, second) =>
                {
                    if (first == "")
                        return second;
                    else if (second == "")
                        return first;
                    else
                        return first + service._separatorChar + second;
                }).ToList();
            }

            return result;
        }

        private List<ModuleInfo> BuildSubmodules(ModuleBuilder parent, CommandService service)
        {
            var result = new List<ModuleInfo>();

            foreach (var submodule in parent.Modules)
                result.Add(submodule.Build(service, this));

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
