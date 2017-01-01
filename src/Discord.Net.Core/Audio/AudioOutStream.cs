using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public abstract class AudioOutStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public virtual void Clear() { }
        public virtual Task ClearAsync(CancellationToken cancelToken) { return Task.Delay(0); }
    }
}
