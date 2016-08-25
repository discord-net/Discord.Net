namespace Discord
{
    /// <summary> Specifies the type of channel a message was sent to or eceived from. </summary>
    public enum ChannelType
    {
        ///<summary> A text channel </summary>
        Text = 0,
        ///<summary> A direct-message text channel </summary>
        DM = 1,
        ///<summary> A voice channel channel </summary>
        Voice = 2,
        ///<summary> A group channel </summary>
        Group = 3
    }
}
