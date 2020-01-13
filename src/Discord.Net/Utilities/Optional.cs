using System;
using System.Collections.Generic;
using System.Text;
// todo: impl
namespace Discord
{
    public struct Optional<T>
    {
        public static Optional<T> Unspecified => default;

        public bool IsSpecified { get; }
        private readonly T _innerValue;

        public T Value
        {
            get
            {
                if (!IsSpecified)
                    throw new UnspecifiedOptionalException();
                return _innerValue;
            }
        }

        public Optional(T value)
        {
            IsSpecified = true;
            _innerValue = value;
        }

        public override string ToString()
        {
            return $"<Optional IsSpecified={IsSpecified}, Value={(IsSpecified ? Value?.ToString() ?? "null" : "(unspecified)")}>";
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional<T> opt)
            {
                if (IsSpecified && opt.IsSpecified)
                    return Value?.Equals(opt.Value) ?? opt.Value == null;
                return IsSpecified == opt.IsSpecified;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
            => IsSpecified ? Value?.GetHashCode() ?? 0 : 0;

        public static bool operator ==(Optional<T> a, Optional<T> b)
            => a.Equals(b);
        public static bool operator !=(Optional<T> a, Optional<T> b)
            => !a.Equals(b);

        // todo: implement comparing, GetValueOrDefault, hash codes etc
    }


    public class UnspecifiedOptionalException : Exception
    {
        public UnspecifiedOptionalException() : base("An attempt was made to access an unspecified optional value") { }
    }
}
