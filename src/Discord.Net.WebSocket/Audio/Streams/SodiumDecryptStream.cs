using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Decrypts an RTP frame using libsodium </summary>
    public class SodiumDecryptStream : AudioOutStream
    {
        private readonly AudioClient _client;
        private readonly AudioStream _next;
        private readonly byte[] _nonce;
        private readonly GCHandle _nonceHandle;
        private readonly IntPtr _noncePtr;

        private bool _isDisposed;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public SodiumDecryptStream(AudioStream next, IAudioClient client)
        {
            _next = next;
            _client = (AudioClient)client;
            _nonce = new byte[24];
            _nonceHandle = GCHandle.Alloc(_nonce, GCHandleType.Pinned);
            _noncePtr = _nonceHandle.AddrOfPinnedObject();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !_isDisposed)
            {
                _nonceHandle.Free();
                _isDisposed = true;
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (_client.SecretKeyPtr == null)
                return;

            Buffer.BlockCopy(buffer, 0, _nonce, 0, 12); //Copy RTP header to nonce
            unsafe
            {
                fixed (byte* ptr = buffer)
                    count = SecretBox.Decrypt(ptr, offset + 12, count - 12, ptr, offset + 12, (byte*)_noncePtr, (byte*)_client.SecretKeyPtr);
            }
            await _next.WriteAsync(buffer, 0, count + 12, cancelToken).ConfigureAwait(false);
        }

        public override async Task FlushAsync(CancellationToken cancelToken)
        {
            await _next.FlushAsync(cancelToken).ConfigureAwait(false);
        }
        public override async Task ClearAsync(CancellationToken cancelToken)
        {
            await _next.ClearAsync(cancelToken).ConfigureAwait(false);
        }
    }
}
