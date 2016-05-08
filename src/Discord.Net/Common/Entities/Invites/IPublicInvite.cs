namespace Discord
{
    public interface IPublicInvite : IInvite
    {
        /// <summary> Gets the name of the the channel this invite is linked to. </summary>
        string ChannelName { get; }
        /// <summary> Gets the name of the guild this invite is linked to. </summary>
        string GuildName { get; }
    }
}