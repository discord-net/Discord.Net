using Discord.LibDave.Binding;

namespace Discord.LibDave;

public sealed class DaveWelcomeResult(WelcomeResultHandle handle) :  IRosterProvider
{
    public bool IsNull => handle is 0;

    public unsafe AllocBuffer<ulong> GetRosterMemberIds()
    {
        ulong* ptr;
        nint length;

        libdave.WelcomeResultGetRosterMemberIds(
            handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    public unsafe AllocBuffer<byte> GetRosterMemberSignature(ulong rosterId)
    {
        byte* ptr;
        nint length;

        libdave.WelcomeResultGetRosterMemberSignature(
            handle,
            rosterId,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    public void Dispose()
    {
        libdave.WelcomeResultDestroy(handle);
    }
}
