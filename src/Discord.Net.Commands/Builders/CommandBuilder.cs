using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class CommandBuilder
    {
        private readonly List<string> _aliases;
        private readonly List<OverloadBuilder> _overloads;

        public ModuleBuilder Module { get; }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }

        public IReadOnlyList<string> Aliases => _aliases;
        public IReadOnlyList<OverloadBuilder> Overloads => _overloads;

        //Automatic
        internal CommandBuilder(ModuleBuilder module)
        {
            Module = module;

            _aliases = new List<string>();
            _overloads = new List<OverloadBuilder>();
        }
        //User-defined
        internal CommandBuilder(ModuleBuilder module, string primaryAlias, Action<OverloadBuilder> defaultOverloadBuilder)
            : this(module)
        {
            Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
            Preconditions.NotNull(defaultOverloadBuilder, nameof(defaultOverloadBuilder));
            
            _aliases.Add(primaryAlias);
            AddOverload(defaultOverloadBuilder);
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
            _aliases.AddRange(aliases);
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
            //Default name to first alias
            if (Name == null)
                Name = _aliases[0];

            return new CommandInfo(this, info, service);
        }
    }
}