using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the maximum length allowed for a string type parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class MaxLengthAttribute : Attribute
    {
        /// <summary>
        ///     Gets the maximum length allowed for a string type parameter.
        /// </summary>
        public int Length { get; }

        /// <summary>
        ///     Sets the maximum length allowed for a string type parameter.
        /// </summary>
        /// <param name="lenght">Maximum string length allowed.</param>
        public MaxLengthAttribute(int lenght)
        {
            Length = lenght;
        }
    }
}
