using System;
using System.Linq;
using System.Reflection;

using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class ParameterBuilder
    {
        private readonly List<ParameterPreconditionAttribute> _preconditions; 

        public CommandBuilder Command { get; }
        public string Name { get; internal set; }
        public Type ParameterType { get; internal set; }

        public TypeReader TypeReader { get; set; }
        public bool IsOptional { get; set; }
        public bool IsRemainder { get; set; }
        public bool IsMultiple { get; set; }
        public object DefaultValue { get; set; }
        public string Summary { get; set; }

        public IReadOnlyList<ParameterPreconditionAttribute> Preconditions => _preconditions;

        //Automatic
        internal ParameterBuilder(CommandBuilder command)
        {
            _preconditions = new List<ParameterPreconditionAttribute>();

            Command = command;
        }
        //User-defined
        internal ParameterBuilder(CommandBuilder command, string name, Type type)
            : this(command)
        {
            Discord.Preconditions.NotNull(name, nameof(name));

            Name = name;
            SetType(type);
        }

        internal void SetType(Type type)
        {
            var readers = Command.Module.Service.GetTypeReaders(type);
            if (readers != null)
                TypeReader = readers.FirstOrDefault().Value;
            else
                TypeReader = Command.Module.Service.GetDefaultTypeReader(type);

            if (TypeReader == null)
                throw new InvalidOperationException($"{type} does not have a TypeReader registered for it");            

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

        public ParameterBuilder AddPrecondition(ParameterPreconditionAttribute precondition)
        {
            _preconditions.Add(precondition);
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