using System;

using System.Reflection;

namespace Discord.Commands
{
    /// <summary> Marks the <see cref="Type"/> to be read by the specified <see cref="TypeReader"/>. </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class OverrideTypeReaderAttribute : Attribute
    {
        private static readonly TypeInfo _typeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

        /// <summary> Gets the specified <see cref="TypeReader"/> of the parameter. </summary>
        public Type TypeReader { get; }

        /// <summary> Marks the parameter to be read with the specified <see cref="TypeReader"/>. </summary>
        /// <param name="overridenTypeReader">The <see cref="TypeReader"/> to be used with the parameter. </param>
        public OverrideTypeReaderAttribute(Type overridenTypeReader)
        {
            if (!_typeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
                throw new ArgumentException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}.");
            
            TypeReader = overridenTypeReader;
        }
    } 
}
