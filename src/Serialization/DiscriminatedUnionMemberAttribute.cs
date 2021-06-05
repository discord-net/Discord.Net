using System;

namespace Discord.Net.Serialization
{
    /// <summary>
    /// Defines an attribute used to mark members of discriminated unions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public class DiscriminatedUnionMemberAttribute : Attribute
    {
        /// <summary>
        /// Gets the discriminator value used to identify this member type.
        /// </summary>
        public object Discriminator { get; }

        /// <summary>
        /// Creates a new <see cref="DiscriminatedUnionMemberAttribute"/>
        /// instance.
        /// </summary>
        /// <param name="discriminator">
        /// The discriminator value used to identify this member type.
        /// </param>
        public DiscriminatedUnionMemberAttribute(object discriminator)
        {
            Discriminator = discriminator;
        }
    }
}
