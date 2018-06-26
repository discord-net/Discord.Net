using System;

using System.Reflection;

namespace Discord.Commands
{
    /// <summary>
    ///     Marks the <see cref="Type"/> to be read by the specified <see cref="TypeReader"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class OverrideTypeReaderAttribute : Attribute
    {
        private static readonly TypeInfo TypeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

        /// <summary> 
        ///     Gets the specified <see cref="TypeReader"/> of the parameter. 
        /// </summary>
        public Type TypeReader { get; }

        /// <inheritdoc/>
        /// <param name="overridenTypeReader">The <see cref="TypeReader"/> to be used with the parameter. </param>
        /// <exception cref="ArgumentException">The given <paramref name="overridenTypeReader"/> does not inherit from <see cref="TypeReader"/>.</exception>
        public OverrideTypeReaderAttribute(Type overridenTypeReader)
        {
            if (!TypeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
                throw new ArgumentException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}.");
            
            TypeReader = overridenTypeReader;
        }
    } 
}
