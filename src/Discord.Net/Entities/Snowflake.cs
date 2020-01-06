namespace Discord
{
    /// <summary>
    /// A Snowflake represents a unique, 64-bit identifier.
    /// </summary>
    public struct Snowflake
    {
        private readonly ulong _value;

        private Snowflake(ulong value)
        {
            _value = value;
        }

        public static implicit operator ulong(Snowflake snowflake)
        {
            return snowflake._value;
        }
        public static implicit operator Snowflake(ulong value)
        {
            return new Snowflake(value);
        }
    }
}
