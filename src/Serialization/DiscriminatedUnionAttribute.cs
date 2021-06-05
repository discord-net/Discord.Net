using System;

namespace Discord.Net.Serialization
{
    /// <summary>
    /// Defines an attribute used to mark discriminated unions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public class DiscriminatedUnionAttribute : Attribute
    {
        /// <summary>
        /// Gets the field or property used to discriminate between types.
        /// </summary>
        public string DiscriminatorField { get; }

        /// <summary>
        /// Creates a new <see cref="DiscriminatedUnionAttribute"/> instance.
        /// </summary>
        /// <param name="discriminatorField">
        /// The field or property used to discriminate between types.
        /// </param>
        public DiscriminatedUnionAttribute(string discriminatorField)
        {
            DiscriminatorField = discriminatorField;
        }
    }
}
