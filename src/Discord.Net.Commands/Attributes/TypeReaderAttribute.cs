using System;
using System.Reflection;

namespace Discord.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true)]
    public class TypeReaderAttribute : Attribute
    {
        public Type Type { get; }
        public TypeInfo OverridingTypeReader { get; }

        public TypeReaderAttribute(Type forType, Type typeReader)
        {
            if (!typeof(TypeReader).GetTypeInfo().IsAssignableFrom(typeReader.GetTypeInfo()))
                throw new ArgumentException($"Type of argument {nameof(typeReader)} must derive from {nameof(TypeReader)}", nameof(typeReader));

            Type = forType;
            OverridingTypeReader = typeReader.GetTypeInfo();
        }
    }
}
