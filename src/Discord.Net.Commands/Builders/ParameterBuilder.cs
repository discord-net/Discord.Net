using System;
using System.Reflection;

namespace Discord.Commands.Builders
{
    public class ParameterBuilder
    {
        public CommandBuilder Command { get; }
        public string Name { get; internal set; }
        public Type ParameterType { get; internal set; }

        public TypeReader TypeReader { get; set; }
        public bool IsOptional { get; set; }
        public bool IsRemainder { get; set; }
        public bool IsMultiple { get; set; }
        public object DefaultValue { get; set; }
        public string Summary { get; set; }

        //Automatic
        internal ParameterBuilder(CommandBuilder command)
        {
            Command = command;
        }
        //User-defined
        internal ParameterBuilder(CommandBuilder command, string name, Type type)
            : this(command)
        {
            Preconditions.NotNull(name, nameof(name));

            Name = name;
            SetType(type);
        }

        internal void SetType(Type type)
        {
            TypeReader = Command.Module.Service.GetTypeReader(type);

            if (type.GetTypeInfo().IsValueType)
                DefaultValue = Activator.CreateInstance(type);
            else if (type.IsArray)
                type = ParameterType.GetElementType();
            ParameterType = type;
        }
        
        public ParameterBuilder WithSummary(string summary)
        {
            Summary = summary;
            return this;
        }
        public ParameterBuilder WithDefault(object defaultValue)
        {
            DefaultValue = defaultValue;            
            return this;
        }
        public ParameterBuilder WithIsOptional(bool isOptional)
        {
            IsOptional = isOptional;
            return this;
        }
        public ParameterBuilder WithIsRemainder(bool isRemainder)
        {
            IsRemainder = isRemainder;
            return this;
        }
        public ParameterBuilder WithIsMultiple(bool isMultiple)
        {
            IsMultiple = isMultiple;
            return this;
        }

        internal ParameterInfo Build(CommandInfo info)
        {
            if (TypeReader == null)
                throw new InvalidOperationException($"No default TypeReader found, one must be specified");

            return new ParameterInfo(this, info, Command.Module.Service);
        }
    }
}