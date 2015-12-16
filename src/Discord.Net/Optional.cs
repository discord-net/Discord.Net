using System.Collections.Generic;

namespace Discord
{
    /*public struct Optional<T>
    {
        public bool HasValue { get; }
        public T Value { get; }
        
        public Optional(T value)
        {
            HasValue = true;
            Value = value;
        }

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        public static bool operator ==(Optional<T> a, Optional<T> b) => 
            a.HasValue == b.HasValue && EqualityComparer<T>.Default.Equals(a.Value, b.Value);
        public static bool operator !=(Optional<T> a, Optional<T> b) => 
            a.HasValue != b.HasValue || EqualityComparer<T>.Default.Equals(a.Value, b.Value);
        public override bool Equals(object obj) => 
            this == ((Optional<T>)obj);
        public override int GetHashCode() => 
            unchecked(HasValue.GetHashCode() + Value?.GetHashCode() ?? 0);

        public override string ToString() => Value?.ToString() ?? "null";
    }*/
}
