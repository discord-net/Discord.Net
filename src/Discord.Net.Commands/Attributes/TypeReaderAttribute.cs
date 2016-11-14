using System;
using System.Reflection;

namespace Discord.Commands
{
    /// <summary>
    /// Allows to override the <see cref="TypeReader"/> used for a parameter/type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true)]
    public class TypeReaderAttribute : Attribute
    {
        /// <summary>
        /// Type of the type that is read.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// <see cref="TypeInfo"/> of the specified <see cref="TypeReader"/>.
        /// </summary>
        public TypeInfo OverridingTypeReader { get; }

        /// <summary>
        /// Specify a <see cref="TypeReader"/> for a particular type.
        /// </summary>
        /// <param name="forType">Type of the type to read.</param>
        /// <param name="typeReader">Type of the <see cref="TypeReader"/> that reads the type.</param>
        public TypeReaderAttribute(Type forType, Type typeReader)
        {
            if (!typeof(TypeReader).GetTypeInfo().IsAssignableFrom(typeReader.GetTypeInfo()))
                throw new ArgumentException($"Type of argument {nameof(typeReader)} must derive from {nameof(TypeReader)}", nameof(typeReader));

            Type = forType;
            OverridingTypeReader = typeReader.GetTypeInfo();
        }
    }
}
