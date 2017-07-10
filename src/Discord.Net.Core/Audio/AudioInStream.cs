using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public abstract class AudioInStream : AudioStream
    {
        public abstract int AvailableFrames { get; }

        public override bool CanRead => true;
        public override bool CanWrite => true;        

        public abstract Task<RTPFrame> ReadFrameAsync(CancellationToken cancelToken);
        public abstract bool TryReadFrame(CancellationToken cancelToken, out RTPFrame frame);

        public override Task FlushAsync(CancellationToken cancelToken) { throw new NotSupportedException(); }
    }
}
