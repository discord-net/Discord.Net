using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Commands.Builders
{
    public class ParameterBuilder
    {
        public ParameterBuilder()
        { }

        public ParameterBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Summary { get; set; }
        public object DefaultValue { get; set; }
        public Type ParameterType { get; set; }

        public TypeReader TypeReader { get; set; }

        public bool Optional { get; set; }
        public bool Remainder { get; set; }
        public bool Multiple { get; set; }

        public ParameterBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public ParameterBuilder SetSummary(string summary)
        {
            Summary = summary;
            return this;
        }

        public ParameterBuilder SetDefault<T>(T defaultValue)
        {
            Optional = true;
            DefaultValue = defaultValue;
            ParameterType = typeof(T);

            if (ParameterType.IsArray)
                ParameterType = ParameterType.GetElementType();

            return this;
        }

        public ParameterBuilder SetType(Type parameterType)
        {
            ParameterType = parameterType; 
            return this;
        }

        public ParameterBuilder SetTypeReader(TypeReader reader)
        {
            TypeReader = reader;
            return this;
        }

        public ParameterBuilder SetOptional(bool isOptional)
        {
            Optional = isOptional;
            return this;
        }

        public ParameterBuilder SetRemainder(bool isRemainder)
        {
            Remainder = isRemainder;
            return this;
        }

        public ParameterBuilder SetMultiple(bool isMultiple)
        {
            Multiple = isMultiple;
            return this;
        }

        internal ParameterInfo Build(CommandInfo info, CommandService service)
        {
            if (TypeReader == null)
                TypeReader = service.GetTypeReader(ParameterType);

            return new ParameterInfo(this, info, service);
        }
    }
}