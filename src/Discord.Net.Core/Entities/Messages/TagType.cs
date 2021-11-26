namespace Discord
{
    /// <summary> Specifies the type of Discord tag. </summary>
    public enum TagType
    {
        /// <summary> The object is an user mention. </summary>
        UserMention,
        /// <summary> The object is a channel mention. </summary>
        ChannelMention,
        /// <summary> The object is a role mention. </summary>
        RoleMention,
        /// <summary> The object is an everyone mention. </summary>
        EveryoneMention,
        /// <summary> The object is a here mention. </summary>
        HereMention,
        /// <summary> The object is an emoji. </summary>
        Emoji
    }
}
