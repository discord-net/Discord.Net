using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class CommandBuilder
    {
        private List<PreconditionAttribute> preconditions;
        private List<ParameterBuilder> parameters;
        private List<string> aliases;

        internal CommandBuilder(ModuleBuilder module, string prefix)
        {
            preconditions = new List<PreconditionAttribute>();
            parameters = new List<ParameterBuilder>();
            aliases = new List<string>();

            if (prefix != null)
            {
                aliases.Add(prefix);
                Name = prefix;
            }

            Module = module;
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public Func<CommandContext, object[], Task> Callback { get; set; }
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

        public CommandBuilder SetCallback(Func<CommandContext, object[], Task> callback)
        {
            Callback = callback;
            return this;
        }

        public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            preconditions.Add(precondition);
            return this;
        }

        public CommandBuilder AddParameter(ParameterBuilder parameter)
        {
            parameters.Add(parameter);
            return this;
        }

        public CommandBuilder AddAlias(string alias)
        {
            aliases.Add(alias);
            return this;
        }

        internal CommandInfo Build(ModuleInfo info, CommandService service)
        {
            return new CommandInfo(this, info, service);
        }
    }
}