namespace Discord.LibDave;

public interface IRosterProvider : IDisposable
{
    AllocBuffer<byte> GetRosterMemberSignature(ulong id);
    AllocBuffer<ulong> GetRosterMemberIds();
}
