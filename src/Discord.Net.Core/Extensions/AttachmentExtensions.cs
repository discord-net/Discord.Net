namespace Discord
{
    public static class AttachmentExtensions
    {
        /// <summary>
        ///     The prefix applied to files to indicate that it is a spoiler.
        /// </summary>
        public const string SpoilerPrefix = "SPOILER_";
        /// <summary>
        ///     Gets whether the message's attachments are spoilers or not.
        /// </summary>
        public static bool IsSpoiler(this IAttachment attachment)
            => attachment.Filename.StartsWith(SpoilerPrefix);
    }
}
