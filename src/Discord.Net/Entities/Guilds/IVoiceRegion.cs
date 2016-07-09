namespace Discord
{
    public interface IVoiceRegion
    {
        /// <summary> Gets the unique identifier for this voice region. </summary>
        string Id { get; }
        /// <summary> Gets the name of this voice region. </summary>
        string Name { get; }
        /// <summary> Returns true if this voice region is exclusive to VIP accounts. </summary>
        bool IsVip { get; }
        /// <summary> Returns true if this voice region is the closest to your machine. </summary>
        bool IsOptimal { get; }
        /// <summary> Gets an example hostname for this voice region. </summary>
        string SampleHostname { get; }
        /// <summary> Gets an example port for this voice region. </summary>
        int SamplePort { get; }
    }
}