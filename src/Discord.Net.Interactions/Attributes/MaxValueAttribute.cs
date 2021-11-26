using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the maximum value permitted for a number type parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class MaxValueAttribute : Attribute
    {
        /// <summary>
        ///     Gets the maximum value permitted.
        /// </summary>
        public double Value { get; }

        /// <summary>
        ///     Set the maximum value permitted for a number type parameter.
        /// </summary>
        /// <param name="value">The maximum value permitted.</param>
        public MaxValueAttribute(double value)
        {
            Value = value;
        }
    }
}
