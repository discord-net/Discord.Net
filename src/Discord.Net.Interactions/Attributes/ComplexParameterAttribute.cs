using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Registers a parameter as a complex parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ComplexParameterAttribute : Attribute
    {
        /// <summary>
        ///     Gets the parameter array of the constructor method that should be prioritized.
        /// </summary>
        public Type[] PrioritizedCtorSignature { get; }

        /// <summary>
        ///     Registers a slash command parameter as a complex parameter.
        /// </summary>
        public ComplexParameterAttribute() { }

        /// <summary>
        ///     Registers a slash command parameter as a complex parameter with a specified constructor signature.
        /// </summary>
        /// <param name="types">Type array of the preferred constructor parameters.</param>
        public ComplexParameterAttribute(Type[] types)
        {
            PrioritizedCtorSignature = types;
        }
    }
}
