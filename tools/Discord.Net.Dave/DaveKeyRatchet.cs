using Discord.LibDave.Binding;

namespace Discord.LibDave;

public sealed class DaveKeyRatchet(KeyRatchetHandle handle) : IDisposable
{
    public KeyRatchetHandle Handle { get; } = handle;

    public void Dispose()
    {
        libdave.KeyRatchetDestroy(Handle);
    }
}
