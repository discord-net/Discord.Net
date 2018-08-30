using System;
using System.Linq;
using System.Reflection;

using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class ParameterBuilder
    {
        private readonly List<ParameterPreconditionAttribute> _preconditions;
        private readonly List<Attribute> _attributes;

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
        public IReadOnlyList<Attribute> Attributes => _attributes;

        //Automatic
        internal ParameterBuilder(CommandBuilder command)
        {
            _preconditions = new List<ParameterPreconditionAttribute>();
            _attributes = new List<Attribute>();

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
            TypeReader = GetReader(type);

            if (type.GetTypeInfo().IsValueType)
                DefaultValue = Activator.CreateInstance(type);
            else if (type.IsArray)
                type = ParameterType.GetElementType();
            ParameterType = type;
        }

        private TypeReader GetReader(Type type)
        {
            var readers = Command.Module.Service.GetTypeReaders(type);
            if (readers != null)
                return readers.FirstOrDefault().Value;
            else
                return Command.Module.Service.GetDefaultTypeReader(type);
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

        public ParameterBuilder AddAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }
        public ParameterBuilder AddPrecondition(ParameterPreconditionAttribute precondition)
        {
            _preconditions.Add(precondition);
            return this;
        }

        internal ParameterInfo Build(CommandInfo info)
        {
            if ((TypeReader ?? (TypeReader = GetReader(ParameterType))) == null)
                throw new InvalidOperationException($"No type reader found for type {ParameterType.Name}, one must be specified");

            return new ParameterInfo(this, info, Command.Module.Service);
        }
    }
}
