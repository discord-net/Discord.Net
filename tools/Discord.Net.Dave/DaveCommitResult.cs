using Discord.LibDave.Binding;

namespace Discord.LibDave;

public sealed class DaveCommitResult(nuint handle) : IRosterProvider
{
    public bool IsFailed => libdave.CommitResultIsFailed(handle);
    public bool IsIgnored => libdave.CommitResultIsIgnored(handle);

    public unsafe AllocBuffer<ulong> GetRosterMemberIds()
    {
        nuint length;
        ulong* ptr;

        libdave.CommitResultGetRosterMemberIds(
            handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    public unsafe AllocBuffer<byte> GetRosterMemberSignature(ulong rosterId)
    {
        nint length;
        byte* ptr;

        libdave.CommitResultGetRosterMemberSignature(
            handle,
            rosterId,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    public void Dispose() => libdave.CommitResultDestroy(handle);
}
