using System;

using System.Reflection;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OverrideTypeReaderAttribute : Attribute
    {
        private readonly TypeInfo _typeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

        public Type TypeReader { get; }

        public OverrideTypeReaderAttribute(Type overridenType)
        {
            if (!_typeReaderTypeInfo.IsAssignableFrom(overridenType.GetTypeInfo()))
                throw new ArgumentException($"{nameof(overridenType)} must inherit from {nameof(TypeReader)}");
            
            TypeReader = overridenType;
        }
    } 
}