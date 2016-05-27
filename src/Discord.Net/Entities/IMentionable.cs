namespace Discord
{
    public interface IMentionable
    {
        /// <summary> Returns a special string used to mention this object.  </summary>
        string Mention { get; }
    }
}
