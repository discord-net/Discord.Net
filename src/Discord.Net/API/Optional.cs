namespace Discord.API
{
    public struct Optional<T> : IOptional
    {
        /// <summary> Gets the value for this paramter, or default(T) if unspecified. </summary>
        public T Value { get; }
        /// <summary> Returns true if this value has been specified. </summary>
        public bool IsSpecified { get; }

        object IOptional.Value => Value;

        /// <summary> Creates a new Parameter with the provided value. </summary>
        public Optional(T value)
        {
            Value = value;
            IsSpecified = true;
        }

        /// <summary> Implicitly creates a new Parameter from an existing value. </summary>
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        /// <summary> Implicitly creates a new Parameter from an existing value. </summary>
        public static implicit operator T(Optional<T> param) => param.Value;

        public override string ToString() => IsSpecified ? (Value?.ToString() ?? null) : null;
    }
}
