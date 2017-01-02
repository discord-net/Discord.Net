namespace Discord
{
    public enum VerificationLevel
    {
        /// <summary> Users have no additional restrictions on sending messages to this guild. </summary>
        None = 0,
        /// <summary> Users must have a verified email on their account. </summary>
        Low = 1,
        /// <summary> Users must fulfill the requirements of Low, and be registered on Discord for at least 5 minutes. </summary>
        Medium = 2,
        /// <summary> Users must fulfill the requirements of Medium, and be a member of this guild for at least 10 minutes. </summary>
        High = 3
    }
}
