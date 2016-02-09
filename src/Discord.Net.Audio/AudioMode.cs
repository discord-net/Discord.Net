namespace Discord.Audio
{
    public enum AudioMode : byte
    {
        Outgoing = 1,
        Incoming = 2,
        Both = Outgoing | Incoming
    }
}
