using System;
using System.Diagnostics;

namespace Discord
{
    //Based on https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Nullable.cs
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Optional<T>
    {
        public static Optional<T> Unspecified => default(Optional<T>);
        private readonly T _value;

        /// <summary> Gets the value for this paramter. </summary>
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

        /// <summary> Creates a new Parameter with the provided value. </summary>
        public Optional(T value)
        {
            _value = value;
            IsSpecified = true;
        }

        public T GetValueOrDefault() => _value;
        public T GetValueOrDefault(T defaultValue) => IsSpecified ? _value : defaultValue;

        public override bool Equals(object other)
        {
            if (!IsSpecified) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }
        public override int GetHashCode() => IsSpecified ? _value.GetHashCode() : 0;

        public override string ToString() => IsSpecified ? _value?.ToString() : null;
        private string DebuggerDisplay => IsSpecified ? (_value?.ToString() ?? "<null>") : "<unspecified>";

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        public static explicit operator T(Optional<T> value) => value.Value;
    }
    public static class Optional
    {
        public static Optional<T> Create<T>() => Optional<T>.Unspecified;
        public static Optional<T> Create<T>(T value) => new Optional<T>(value);

        public static T? ToNullable<T>(this Optional<T> val)
            where T : struct
            => val.IsSpecified ? val.Value : (T?)null;
    }
}
