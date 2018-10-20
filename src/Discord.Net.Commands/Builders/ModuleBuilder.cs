using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands.Builders
{
    public class ModuleBuilder
    {
        private readonly List<CommandBuilder> _commands;
        private readonly List<ModuleBuilder> _submodules;
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<Attribute> _attributes;
        private readonly List<string> _aliases;

        public CommandService Service { get; }
        public ModuleBuilder Parent { get; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public string Group { get; set; }

        public IReadOnlyList<CommandBuilder> Commands => _commands;
        public IReadOnlyList<ModuleBuilder> Modules => _submodules;
        public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<string> Aliases => _aliases;

        internal TypeInfo TypeInfo { get; set; }

        //Automatic
        internal ModuleBuilder(CommandService service, ModuleBuilder parent)
        {
            Service = service;
            Parent = parent;

            _commands = new List<CommandBuilder>();
            _submodules = new List<ModuleBuilder>();
            _preconditions = new List<PreconditionAttribute>();
            _attributes = new List<Attribute>();
            _aliases = new List<string>();
        }
        //User-defined
        internal ModuleBuilder(CommandService service, ModuleBuilder parent, string primaryAlias)
            : this(service, parent)
        {
            Discord.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));

            _aliases = new List<string> { primaryAlias };
        }

        public ModuleBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
        public ModuleBuilder WithSummary(string summary)
        {
            Summary = summary;
            return this;
        }
        public ModuleBuilder WithRemarks(string remarks)
        {
            Remarks = remarks;
            return this;
        }

        public ModuleBuilder AddAliases(params string[] aliases)
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                string alias = aliases[i] ?? "";
                if (!_aliases.Contains(alias))
                    _aliases.Add(alias);
            }
            return this;
        }
        public ModuleBuilder AddAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }
        public ModuleBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            _preconditions.Add(precondition);
            return this;
        }
        public ModuleBuilder AddCommand(string primaryAlias, Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> callback, Action<CommandBuilder> createFunc)
        {
            var builder = new CommandBuilder(this, primaryAlias, callback);
            createFunc(builder);
            _commands.Add(builder);
            return this;
        }
        internal ModuleBuilder AddCommand(Action<CommandBuilder> createFunc)
        {
            var builder = new CommandBuilder(this);
            createFunc(builder);
            _commands.Add(builder);
            return this;
        }
        public ModuleBuilder AddModule(string primaryAlias, Action<ModuleBuilder> createFunc)
        {
            var builder = new ModuleBuilder(Service, this, primaryAlias);
            createFunc(builder);
            _submodules.Add(builder);
            return this;
        }
        internal ModuleBuilder AddModule(Action<ModuleBuilder> createFunc)
        {
            var builder = new ModuleBuilder(Service, this);
            createFunc(builder);
            _submodules.Add(builder);
            return this;
        }

        private ModuleInfo BuildImpl(CommandService service, IServiceProvider services, ModuleInfo parent = null)
        {
            //Default name to first alias
            if (Name == null)
                Name = _aliases[0];

            if (TypeInfo != null && !TypeInfo.IsAbstract)
            {
                var moduleInstance = ReflectionUtils.CreateObject<IModuleBase>(TypeInfo, service, services);
                moduleInstance.OnModuleBuilding(service, this);
            }

            return new ModuleInfo(this, service, services, parent);
        }

        public ModuleInfo Build(CommandService service, IServiceProvider services) => BuildImpl(service, services);

        internal ModuleInfo Build(CommandService service, IServiceProvider services, ModuleInfo parent) => BuildImpl(service, services, parent);
    }
}
