namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildRolePositionsParams
    {
        /// <summary>
        /// Role.
        /// </summary>
        public Snowflake Id { get; set; }

        /// <summary>
        /// Sorting position of the role.
        /// </summary>
        public Optional<int?> Position { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
