namespace Discord
{
    public static class AttachmentExtensions
    {
        /// <summary>
        ///     Gets whether the message's attachments are spoilers or not.
        /// </summary>
        public static bool IsSpoiler(this IAttachment attachment)
            => attachment.Filename.StartsWith(IAttachment.SpoilerPrefix);
    }
}
