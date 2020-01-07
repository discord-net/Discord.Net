namespace Discord.Models
{
    public enum VerificationLevel : byte
    {
        /// <summary>
        /// Unrestricted
        /// </summary>
        None = 0,

        /// <summary>
        /// Account must have a verified email
        /// </summary>
        Low = 1,

        /// <summary>
        /// Account must be registered on Discord for more than 5 minutes
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Must be a member of the server for more than 10 minutes
        /// </summary>
        High = 3,

        /// <summary>
        /// Must have a verified phone number
        /// </summary>
        VeryHigh = 4
    }
}
