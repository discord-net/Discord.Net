using System;

namespace Discord.Net
{
    /// <summary>
    ///     Container to keep a type that might not be present.
    /// </summary>
    /// <typeparam name="T">Inner type</typeparam>
    public struct Optional<T>
    {
        private readonly T _value;

        /// <summary>
        ///     Gets the inner value of this <see cref="Optional{T}"/> if present.
        /// </summary>
        /// <returns>The value inside this <see cref="Optional{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">This <see cref="Optional{T}"/> has no inner value.</exception>
        public T Value => !IsSpecified ? throw new InvalidOperationException("This property has no value set.") : _value;

        /// <summary>
        ///     Gets if this <see cref="Optional{T}"/> has an inner value.
        /// </summary>
        /// <returns>A boolean that determines if this <see cref="Optional{T}"/> has a <see cref="Value"/>.</returns>
        public bool IsSpecified { get; }

        private Optional(T value)
        {
            _value = value;
            IsSpecified = true;
        }

        /// <summary>
        ///     Creates a new unspecified <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns>An unspecified <see cref="Optional{T}"/>.</returns>
        public static Optional<T> Create()
            => default;

        /// <summary>
        ///     Creates a new <see cref="Optional{T}"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value that will be specified for this <see cref="Optional{T}"/>.</param>
        /// <returns>A specified <see cref="Optional{T}"/> with the provided value inside.</returns>
        public static Optional<T> Create(T value)
            => new(value);

        /// <summary>
        ///     Gets the <see cref="Value"/> or their <see langword="default"/> value.
        /// </summary>
        /// <returns>The value inside this <see cref="Optional{T}"/> or their <see langword="default"/> value.</returns>
        public T GetValueOrDefault()
            => _value;

        /// <summary>
        ///     Gets the <see cref="Value"/> or the default value provided.
        /// </summary>
        /// <returns>The value inside this <see cref="Optional{T}"/> or default value provided.</returns>
        public T GetValueOrDefault(T defaultValue)
            => IsSpecified ? _value : defaultValue;

        /// <inheritdoc/>
        public override bool Equals(object? other)
        {
            if (!IsSpecified)
                return other == null;
            if (other == null || _value == null)
                return false;
            return _value.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => IsSpecified ? _value?.GetHashCode() ?? default : default;

        /// <summary>
        ///     Returns the inner value ToString value or this type fully qualified name.
        /// </summary>
        /// <returns>The inner value string value or this type fully qualified name.</returns>
        public override string? ToString()
            => IsSpecified ? _value?.ToString() : default;

        /// <summary>
        ///     Creates a new <see cref="Optional{T}"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>A new <see cref="Optional{T}"/> with the specified <paramref name="value"/></returns>
        public static implicit operator Optional<T>(T value)
            => new(value);

        /// <summary>
        ///     Gets the inner value.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>The inner value</returns>
        public static explicit operator T(Optional<T> value)
            => value.Value;
    }
}
