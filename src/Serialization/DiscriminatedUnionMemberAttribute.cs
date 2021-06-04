using System;

namespace Discord.Net.Serialization
{
    /// <summary>
    /// Defines an attribute used to mark members of discriminated unions.
    /// </summary>
    public class DiscriminatedUnionMemberAttribute : Attribute
    {
        /// <summary>
        /// Gets the discriminator value used to identify this member type.
        /// </summary>
        public string Discriminator { get; }

        /// <summary>
        /// Creates a new <see cref="DiscriminatedUnionMemberAttribute"/>
        /// instance.
        /// </summary>
        /// <param name="discriminator">
        /// The discriminator value used to identify this member type.
        /// </param>
        public DiscriminatedUnionMemberAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }
    }
}
