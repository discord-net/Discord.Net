namespace Discord
{
    /// <summary> Represents whether the object is mentionable or not. </summary>
    public interface IMentionable
    {
        /// <summary> Returns a special string used to mention this object.  </summary>
        string Mention { get; }
    }
}
