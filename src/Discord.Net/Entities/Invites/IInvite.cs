namespace Discord
{
    public interface IInvite : IEntity<string>
    {
        /// <summary> Gets the unique code for this invite. </summary>
        string Code { get; }
        /// <summary> Gets, if enabled, an alternative human-readable invite code. </summary>
        string XkcdCode { get; }

        /// <summary> Returns a URL for this invite using Code. </summary>
        string Url { get; }
        /// <summary> Returns a URL for this invite using XkcdCode if available or null if not. </summary>
        string XkcdUrl { get; }

        /// <summary> Gets information about the guild this invite is attached to. </summary>
        InviteGuild Guild { get; }
        /// <summary> Gets information about the channel this invite is attached to. </summary>
        InviteChannel Channel { get; }
    }
}
