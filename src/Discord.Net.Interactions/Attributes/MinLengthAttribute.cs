using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the minimum length allowed for a string type parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class MinLengthAttribute : Attribute
    {
        /// <summary>
        ///     Gets the minimum length allowed for a string type parameter.
        /// </summary>
        public int Length { get; }

        /// <summary>
        ///     Sets the minimum length allowed for a string type parameter.
        /// </summary>
        /// <param name="lenght">Minimum string length allowed.</param>
        public MinLengthAttribute(int lenght)
        {
            Length = lenght;
        }
    }
}
