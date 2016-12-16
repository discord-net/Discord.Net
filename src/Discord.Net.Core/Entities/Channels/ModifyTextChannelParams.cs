namespace Discord
{
    /// <inheritdoc />
    public class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        /// <summary>
        /// What the topic of the channel should be set to.
        /// </summary>
        public Optional<string> Topic { get; set; }
    }
}
