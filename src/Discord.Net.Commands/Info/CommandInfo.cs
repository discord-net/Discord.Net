using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    [DebuggerDisplay("{Name,nq}")]
    public class CommandInfo
    {
        public ModuleInfo Module { get; }
        public string Name { get; }
        public string Summary { get; }
        public string Remarks { get; }

        public IReadOnlyList<string> Aliases { get; }
        public IReadOnlyList<OverloadInfo> Overloads { get; }

        internal CommandInfo(CommandBuilder builder, ModuleInfo module, CommandService service)
        {
            Module = module;

            Name = builder.Name;
            Summary = builder.Summary;
            Remarks = builder.Remarks;

            // both command and module provide aliases
            if (module.Aliases.Count > 0 && builder.Aliases.Count > 0)
                Aliases = module.Aliases.Permutate(builder.Aliases, (first, second) => second != null ? first + " " + second : first).Select(x => service._caseSensitive ? x : x.ToLowerInvariant()).ToImmutableArray();
            // only module provides aliases
            else if (module.Aliases.Count > 0)
                Aliases = module.Aliases.Select(x => service._caseSensitive ? x : x.ToLowerInvariant()).ToImmutableArray();
            // only command provides aliases
            else if (builder.Aliases.Count > 0)
                Aliases = builder.Aliases.Select(x => service._caseSensitive ? x : x.ToLowerInvariant()).ToImmutableArray();
            // neither provide aliases
            else
                throw new InvalidOperationException("Cannot build a command without any aliases");

            Overloads = builder.Overloads.Select(x => x.Build(this, service)).ToImmutableArray();
        }
    }
}
