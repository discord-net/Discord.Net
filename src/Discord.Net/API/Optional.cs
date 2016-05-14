using System;

namespace Discord.API
{
    //Based on https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Nullable.cs
    public struct Optional<T> : IOptional
    {
        private readonly T _value;

        /// <summary> Gets the value for this paramter, or default(T) if unspecified. </summary>
        public T Value
        {
            get
            {
                if (!IsSpecified)
                    throw new InvalidOperationException("This property has no value set.");
                return _value;
            }
        }
        /// <summary> Returns true if this value has been specified. </summary>
        public bool IsSpecified { get; }

        object IOptional.Value => _value;

        /// <summary> Creates a new Parameter with the provided value. </summary>
        public Optional(T value)
        {
            _value = value;
            IsSpecified = true;
        }
        
        public T GetValueOrDefault() => _value;
        public T GetValueOrDefault(T defaultValue) => IsSpecified ? _value : default(T);

        public override bool Equals(object other)
        {
            if (!IsSpecified) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }

        public override int GetHashCode() => IsSpecified ? _value.GetHashCode() : 0;
        public override string ToString() => IsSpecified ? _value.ToString() : "";

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        public static implicit operator T(Optional<T> value) => value.Value;
    }
}
