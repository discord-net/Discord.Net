using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class CommandBuilder
    {
        private List<PreconditionAttribute> preconditions;
        private List<ParameterBuilder> parameters;
        private List<string> aliases;

        internal CommandBuilder(ModuleBuilder module)
        {
            preconditions = new List<PreconditionAttribute>();
            parameters = new List<ParameterBuilder>();
            aliases = new List<string>();

            Module = module;
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public RunMode RunMode { get; set; }
        public int Priority { get; set; }
        public Func<CommandContext, object[], IDependencyMap, Task> Callback { get; set; }
        public ModuleBuilder Module { get; }

        public List<PreconditionAttribute> Preconditions => preconditions;
        public List<ParameterBuilder> Parameters => parameters;
        public List<string> Aliases => aliases;

        public CommandBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public CommandBuilder SetSummary(string summary)
        {
            Summary = summary;
            return this;
        }

        public CommandBuilder SetRemarks(string remarks)
        {
            Remarks = remarks;
            return this;
        }

        public CommandBuilder SetRunMode(RunMode runMode)
        {
            RunMode = runMode;
            return this;
        }

        public CommandBuilder SetPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        public CommandBuilder SetCallback(Func<CommandContext, object[], IDependencyMap, Task> callback)
        {
            Callback = callback;
            return this;
        }

        public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            preconditions.Add(precondition);
            return this;
        }

        public CommandBuilder AddParameter(Action<ParameterBuilder> createFunc)
        {
            var param = new ParameterBuilder();
            createFunc(param);
            parameters.Add(param);
            return this;
        }

        public CommandBuilder AddAliases(params string[] newAliases)
        {
            aliases.AddRange(newAliases);
            return this;
        }

        internal CommandInfo Build(ModuleInfo info, CommandService service)
        {
            if (aliases.Count == 0)
                throw new InvalidOperationException("Commands require at least one alias to be registered");

            if (Callback == null)
                throw new InvalidOperationException("Commands require a callback to be built");

            if (Name == null)
                Name = aliases[0];

            if (parameters.Count > 0)
            {
                var lastParam = parameters[parameters.Count - 1];

                var firstMultipleParam = parameters.FirstOrDefault(x => x.Multiple);
                if ((firstMultipleParam != null) && (firstMultipleParam != lastParam))
                    throw new InvalidOperationException("Only the last parameter in a command may have the Multiple flag.");
                
                var firstRemainderParam = parameters.FirstOrDefault(x => x.Remainder);
                if ((firstRemainderParam != null) && (firstRemainderParam != lastParam))
                    throw new InvalidOperationException("Only the last parameter in a command may have the Remainder flag.");
            }

            return new CommandInfo(this, info, service);
        }
    }
}