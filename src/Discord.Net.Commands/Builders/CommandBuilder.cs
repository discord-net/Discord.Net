using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Commands.Builders
{
    public class CommandBuilder
    {
        private readonly List<OverloadBuilder> _overloads;
        private readonly List<string> _aliases;

        public ModuleBuilder Module { get; }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public string PrimaryAlias { get; set; }

        public IReadOnlyList<OverloadBuilder> Overloads => _overloads;
        public IReadOnlyList<string> Aliases => _aliases;

        //Automatic
        internal CommandBuilder(ModuleBuilder module)
        {
            Module = module;

            _overloads = new List<OverloadBuilder>();
            _aliases = new List<string>();
        }
        //User-defined
        internal CommandBuilder(ModuleBuilder module, string primaryAlias, Action<OverloadBuilder> defaultOverloadBuilder)
            : this(module)
        {
            Discord.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
            Discord.Preconditions.NotNull(defaultOverloadBuilder, nameof(defaultOverloadBuilder));

            PrimaryAlias = primaryAlias;
            _aliases.Add(primaryAlias);

            var defaultOverload = new OverloadBuilder(this);
            defaultOverloadBuilder(defaultOverload);

            _overloads.Add(defaultOverload);
        }

        public CommandBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
        public CommandBuilder WithSummary(string summary)
        {
            Summary = summary;
            return this;
        }
        public CommandBuilder WithRemarks(string remarks)
        {
            Remarks = remarks;
            return this;
        }

        public CommandBuilder AddAliases(params string[] aliases)
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                var alias = aliases[i] ?? "";
                if (!_aliases.Contains(alias))
                    _aliases.Add(alias);
            }
            return this;
        }

        public CommandBuilder AddOverload(Action<OverloadBuilder> overloadBuilder)
        {
            var overload = new OverloadBuilder(this);
            overloadBuilder(overload);
            _overloads.Add(overload);
            return this;
        }

        internal CommandInfo Build(ModuleInfo info, CommandService service)
        {
            //Default name to primary alias
            if (Name == null)
                Name = PrimaryAlias;

            return new CommandInfo(this, info, service);
        }
    }
}
