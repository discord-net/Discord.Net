using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Discord.Commands.Builders;

namespace Discord.Commands
{
    /// <summary>
    ///     Provides the information of a module.
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        ///     Gets the command service associated with this module.
        /// </summary>
        public CommandService Service { get; }
        /// <summary>
        ///     Gets the name of this module.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the summary of this module.
        /// </summary>
        public string Summary { get; }
        /// <summary>
        ///     Gets the remarks of this module.
        /// </summary>
        public string Remarks { get; }
        /// <summary>
        ///     Gets the group name (main prefix) of this module.
        /// </summary>
        public string Group { get; }

        /// <summary>
        ///     Gets a read-only list of aliases associated with this module.
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }
        /// <summary>
        ///     Gets a read-only list of commands associated with this module.
        /// </summary>
        public IReadOnlyList<CommandInfo> Commands { get; }
        /// <summary>
        ///     Gets a read-only list of preconditions that apply to this module.
        /// </summary>
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }
        /// <summary>
        ///     Gets a read-only list of attributes that apply to this module.
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }
        /// <summary>
        ///     Gets a read-only list of submodules associated with this module.
        /// </summary>
        public IReadOnlyList<ModuleInfo> Submodules { get; }
        /// <summary>
        ///     Gets the parent module of this submodule if applicable.
        /// </summary>
        public ModuleInfo Parent { get; }
        /// <summary>
        ///     Gets a value that indicates whether this module is a submodule or not.
        /// </summary>
        public bool IsSubmodule => Parent != null;

        internal ModuleInfo(ModuleBuilder builder, CommandService service, IServiceProvider services, ModuleInfo parent = null)
        {
            Service = service;

            Name = builder.Name;
            Summary = builder.Summary;
            Remarks = builder.Remarks;
            Group = builder.Group;
            Parent = parent;

            Aliases = BuildAliases(builder, service).ToImmutableArray();
            Commands = builder.Commands.Select(x => x.Build(this, service)).ToImmutableArray();
            Preconditions = BuildPreconditions(builder).ToImmutableArray();
            Attributes = BuildAttributes(builder).ToImmutableArray();

            Submodules = BuildSubmodules(builder, service, services).ToImmutableArray();
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

        private List<ModuleInfo> BuildSubmodules(ModuleBuilder parent, CommandService service, IServiceProvider services)
        {
            var result = new List<ModuleInfo>();

            foreach (var submodule in parent.Modules)
                result.Add(submodule.Build(service, services, this));

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

        private static List<Attribute> BuildAttributes(ModuleBuilder builder)
        {
            var result = new List<Attribute>();

            ModuleBuilder parent = builder;
            while (parent != null)
            {
                result.AddRange(parent.Attributes);
                parent = parent.Parent;
            }

            return result;
        }
    }
}
