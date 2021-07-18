namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildChannelPositionsParams
    {
        /// <summary>
        /// Channel id.
        /// </summary>
        public Snowflake Id { get; set; }

        /// <summary>
        /// Sorting position of the channel.
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Syncs the permission overwrites with the new parent, if moving to a new category.
        /// </summary>
        public bool? LockPermissions { get; set; }

        /// <summary>
        /// The new parent ID for the channel that is moved.
        /// </summary>
        public Snowflake? ParentId { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
