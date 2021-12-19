using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the minimum value permitted for a number type parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class MinValueAttribute : Attribute
    {
        /// <summary>
        ///     Gets the minimum value permitted.
        /// </summary>
        public double Value { get; }

        /// <summary>
        ///     Set the minimum value permitted for a number type parameter.
        /// </summary>
        /// <param name="value">The minimum value permitted.</param>
        public MinValueAttribute(double value)
        {
            Value = value;
        }
    }
}
