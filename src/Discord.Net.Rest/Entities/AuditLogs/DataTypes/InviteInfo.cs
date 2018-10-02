namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for an invite.
    /// </summary>
    public struct InviteInfo
    {
        internal InviteInfo(int? maxAge, string code, bool? temporary, ulong? channelId, int? maxUses)
        {
            MaxAge = maxAge;
            Code = code;
            Temporary = temporary;
            ChannelId = channelId;
            MaxUses = maxUses;
        }

        /// <summary>
        ///     Gets the time (in seconds) until the invite expires.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds until this invite expires; <c>null</c> if this
        ///     invite never expires or not specified.
        /// </returns>
        public int? MaxAge { get; }
        /// <summary>
        ///     Gets the unique identifier for this invite.
        /// </summary>
        /// <returns>
        ///     A string containing the invite code (e.g. <c>FTqNnyS</c>).
        /// </returns>
        public string Code { get; }
        /// <summary>
        ///     Gets a value that indicates whether the invite is a temporary one.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if users accepting this invite will be removed from the guild when they log off, 
        ///     <c>false</c> if not; <c>null</c> if not specified.
        /// </returns>
        public bool? Temporary { get; }
        /// <summary>
        ///     Gets the ID of the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the channel snowflake identifier that the invite points to; 
        ///     <c>null</c> if not specified.
        /// </returns>
        public ulong? ChannelId { get; }
        /// <summary>
        ///     Gets the max number of uses this invite may have.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of uses this invite may be accepted until it is removed
        ///     from the guild; <c>null</c> if none is specified.
        /// </returns>
        public int? MaxUses { get; }
    }
}
