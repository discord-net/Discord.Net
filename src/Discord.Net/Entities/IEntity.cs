namespace Discord
{
    public interface IEntity<TId>
    {
        /// <summary> Gets the unique identifier for this object. </summary>
        TId Id { get; }

        //TODO: What do we do when an object is destroyed due to reconnect? This summary isn't correct.
        /// <summary> Returns true if this object is getting live updates from the DiscordClient. </summary>
        bool IsAttached { get;}
    }
}
