using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

using CommandCallback = System.Func<Discord.Commands.ICommandContext, object[], System.IServiceProvider, Discord.Commands.OverloadInfo, System.Threading.Tasks.Task>;

namespace Discord.Commands.Builders
{
    public class OverloadBuilder
    {
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<ParameterBuilder> _parameters;

        public CommandBuilder Command { get; }
        public CommandCallback Callback { get; set; }

        public RunMode RunMode { get; set; }
        public int Priority { get; set; }

        public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
        public IReadOnlyList<ParameterBuilder> Parameters => _parameters;

        internal OverloadBuilder(CommandBuilder command)
        {
            Command = command;

            _preconditions = new List<PreconditionAttribute>();
            _parameters = new List<ParameterBuilder>();
        }

        public OverloadBuilder WithRunMode(RunMode runMode)
        {
            RunMode = runMode;
            return this;
        }

        public OverloadBuilder WithPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        public OverloadBuilder WithCallback(CommandCallback callback)
        {
            Callback = callback;
            return this;
        }

        public OverloadBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            _preconditions.Add(precondition);
            return this;
        }
        internal OverloadBuilder AddParameter(Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder(this);
            createFunc(param);
            _parameters.Add(param);
            return this;
        }
        public OverloadBuilder AddParameter(string name, Type type, Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder(this, name, type);
            createFunc(param);
            _parameters.Add(param);
            return this;
        }

        internal OverloadInfo Build(CommandInfo info, CommandService service)
        {
            Discord.Preconditions.NotNull(Callback, nameof(Callback));

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

            return new OverloadInfo(this, info, service);
        }
    }
}