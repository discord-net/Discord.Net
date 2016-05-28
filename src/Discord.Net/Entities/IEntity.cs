namespace Discord
{
    public interface IEntity<TId>
    {
        /// <summary> Gets the unique identifier for this object. </summary>
        TId Id { get; }
    }
}
