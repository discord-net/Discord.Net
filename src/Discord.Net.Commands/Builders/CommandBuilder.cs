using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands
{
    public class CommandBuilder
    {
        private List<string> aliases;

        internal CommandBuilder(ModuleBuilder module, string prefix)
        {
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
        public Func<object[], Task> Callback { get; set; }
        public ModuleBuilder Module { get; }

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

        public CommandBuilder SetCallback(Func<object[], Task> callback)
        {
            Callback = callback;
            return this;
        }

        public CommandBuilder AddAlias(string alias)
        {
            aliases.Add(alias);
            return this;
        }

        public ModuleBuilder Done()
        {
            return Module;
        }
    }
}