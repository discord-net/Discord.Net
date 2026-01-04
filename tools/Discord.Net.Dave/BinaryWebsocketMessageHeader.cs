using System.Runtime.InteropServices;

namespace Discord.LibDave;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct BinaryWebsocketMessageHeader
{
    public readonly ushort SequenceNumber;
    public readonly byte OpCode;
}
