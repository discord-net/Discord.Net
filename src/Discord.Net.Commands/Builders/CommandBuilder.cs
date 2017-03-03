using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class CommandBuilder
    {
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<ParameterBuilder> _parameters;
        private readonly List<string> _aliases;

        public ModuleBuilder Module { get; }
        internal Func<ICommandContext, object[], IDependencyMap, Task> Callback { get; set; }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public RunMode RunMode { get; set; }
        public int Priority { get; set; }

        public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
        public IReadOnlyList<ParameterBuilder> Parameters => _parameters;
        public IReadOnlyList<string> Aliases => _aliases;

        //Automatic
        internal CommandBuilder(ModuleBuilder module)
        {
            Module = module;

            _preconditions = new List<PreconditionAttribute>();
            _parameters = new List<ParameterBuilder>();
            _aliases = new List<string>();
        }
        //User-defined
        internal CommandBuilder(ModuleBuilder module, string primaryAlias, Func<ICommandContext, object[], IDependencyMap, Task> callback)
            : this(module)
        {
            Discord.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
            Discord.Preconditions.NotNull(callback, nameof(callback));

            Callback = callback;
            _aliases.Add(primaryAlias);
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
        public CommandBuilder WithRunMode(RunMode runMode)
        {
            RunMode = runMode;
            return this;
        }
        public CommandBuilder WithPriority(int priority)
        {
            Priority = priority;
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
        public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            _preconditions.Add(precondition);
            return this;
        }
        public CommandBuilder AddParameter<T>(string name, Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder(this, name, typeof(T));
            createFunc(param);
            _parameters.Add(param);
            return this;
        }
        public CommandBuilder AddParameter(string name, Type type, Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder(this, name, type);
            createFunc(param);
            _parameters.Add(param);
            return this;
        }
        internal CommandBuilder AddParameter(Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder(this);
            createFunc(param);
            _parameters.Add(param);
            return this;
        }

        internal CommandInfo Build(ModuleInfo info, CommandService service)
        {
            //Default name to first alias
            if (Name == null)
                Name = _aliases[0];

            if (_parameters.Count > 0)
            {
                var lastParam = _parameters[_parameters.Count - 1];

                var firstMultipleParam = _parameters.FirstOrDefault(x => x.IsMultiple);
                if ((firstMultipleParam != null) && (firstMultipleParam != lastParam))
                    throw new InvalidOperationException("Only the last parameter in a command may have the Multiple flag.");
                
                var firstRemainderParam = _parameters.FirstOrDefault(x => x.IsRemainder);
                if ((firstRemainderParam != null) && (firstRemainderParam != lastParam))
                    throw new InvalidOperationException("Only the last parameter in a command may have the Remainder flag.");
            }

            return new CommandInfo(this, info, service);
        }
    }
}