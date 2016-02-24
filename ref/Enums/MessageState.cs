namespace Discord
{
    public enum MessageState : byte
    {
        /// <summary> Message did not originate from this session, or was successfully sent. </summary>
		Normal = 0,
        /// <summary> Message is current queued. </summary>
		Queued,
        /// <summary> Message was deleted. </summary>
        Deleted,
        /// <summary> Message was deleted before it was sent. </summary>
        Aborted,
        /// <summary> Message failed to be sent. </summary>
		Failed,
        /// <summary> Message has been removed from cache and will no longer receive updates. </summary>
        Detached
    }
}
