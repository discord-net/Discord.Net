using Discord.LibDave.Binding;
using System.Runtime.CompilerServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a wrapper around a native handle to a <see cref="libdave"/> object.
/// </summary>
public interface INativeHandle : IDisposable
{
    /// <summary>
    ///     Gets the underlying pointer this object represents.
    /// </summary>
    nuint UnderlyingHandle { get; }

    /// <summary>
    ///     Gets whether the current object is alive and can be used.
    /// </summary>
    bool IsAlive { get; }
}

/// <summary>
///     A class containing extensions related to <see cref="INativeHandle"/>.
/// </summary>
public static class NativeHandleExtensions
{
    extension<T>(T handle) where T : INativeHandle
    {
        /// <summary>
        ///     Gets whether the underlying handle is null.
        /// </summary>
        public bool IsNull => handle.UnderlyingHandle is 0;

        /// <summary>
        ///     Throws if the underlying not alive.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The underlying handle is no longer alive.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfNotAlive()
        {
            if (handle.IsAlive) return;

            throw new ObjectDisposedException(typeof(T).Name);
        }
    }
}
