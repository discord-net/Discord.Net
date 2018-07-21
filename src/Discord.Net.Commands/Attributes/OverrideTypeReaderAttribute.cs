using System;

using System.Reflection;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class OverrideTypeReaderAttribute : Attribute
    {
        private static readonly TypeInfo TypeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

        public Type TypeReader { get; }

        public OverrideTypeReaderAttribute(Type overridenTypeReader)
        {
            if (!TypeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
                throw new ArgumentException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}");
            
            TypeReader = overridenTypeReader;
        }
    } 
}
