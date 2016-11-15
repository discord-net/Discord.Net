using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands.Builders
{
    public class ModuleBuilder
    {
        private List<CommandBuilder> commands;
        private List<ModuleBuilder> submodules;
        private List<PreconditionAttribute> preconditions;
        private List<string> aliases;

        public ModuleBuilder()
            : this(null)
        { }

        internal ModuleBuilder(ModuleBuilder parent)
        {
            commands = new List<CommandBuilder>();
            submodules = new List<ModuleBuilder>();
            preconditions = new List<PreconditionAttribute>();
            aliases = new List<string>();
            
            ParentModule = parent;
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public ModuleBuilder ParentModule { get; }

        public List<CommandBuilder> Commands => commands;
        public List<ModuleBuilder> Modules => submodules;
        public List<PreconditionAttribute> Preconditions => preconditions;
        public List<string> Aliases => aliases;

        public ModuleBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public ModuleBuilder SetSummary(string summary)
        {
            Summary = summary;
            return this;
        }

        public ModuleBuilder SetRemarks(string remarks)
        {
            Remarks = remarks;
            return this;
        }

        public ModuleBuilder AddAliases(params string[] newAliases)
        {
            aliases.AddRange(newAliases);
            return this;
        }

        public ModuleBuilder AddPrecondition(PreconditionAttribute precondition)
        {
            preconditions.Add(precondition);
            return this;
        }

        public ModuleBuilder AddCommand(Action<CommandBuilder> createFunc)
        {
            var builder = new CommandBuilder(this);
            createFunc(builder);
            commands.Add(builder);
            return this;
        }

        public ModuleBuilder AddSubmodule(Action<ModuleBuilder> createFunc)
        {
            var builder = new ModuleBuilder(this);
            createFunc(builder);
            submodules.Add(builder);
            return this;
        }

        public ModuleInfo Build(CommandService service)
        {
            return new ModuleInfo(this, service);
        }
    }
}
