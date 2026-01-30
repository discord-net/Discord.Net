using Discord.LibDave;
using Discord.LibDave.Binding;
using Discord.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams;

/// <summary>
///     Decrypts audio using <c>libdave</c>.
/// </summary>

public sealed class DaveDecryptStream : AudioOutStream
{
    private readonly IAudioClient _client;
    private readonly DaveDecryptor _decryptor;
    private readonly AudioOutStream _next;
    private readonly Logger _logger;
    private readonly ulong _userId;

    private ushort _seq;
    private uint _timestamp;
    private bool _missed;

    private bool _hasHeader;

    internal DaveDecryptStream(
        IAudioClient client,
        DaveDecryptor decryptor,
        AudioOutStream next,
        Logger logger,
        ulong userId
    )
    {
        _client = client;
        _decryptor = decryptor;
        _next = next;
        _logger = logger;
        _userId = userId;
    }

    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        _seq = seq;
        _timestamp = timestamp;
        _missed = missed;
        _hasHeader = true;
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var result = _decryptor.Decrypt(
            new ReadOnlyMemory<byte>(buffer, offset, count),
            MediaType.Audio,
            out var decryptedBuffer,
            out var decryptedLength
        );

        if (result is not DecryptorResultCode.Success)
        {
            await _logger.WarningAsync($"Failed to decrypt audio packet for {_userId}: {result}");
            decryptedBuffer.Dispose();
            return;
        }

        if (_hasHeader)
        {
            _next.WriteHeader(_seq, _timestamp, _missed);
            _hasHeader = false;
        }

        try
        {
            await _next.WriteAsync(decryptedBuffer.Memory[..decryptedLength], cancellationToken);
        }
        finally
        {
            decryptedBuffer.Dispose();
        }
    }

    public override Task FlushAsync(CancellationToken cancelToken)
        => _next.FlushAsync(cancelToken);

    public override Task ClearAsync(CancellationToken cancelToken)
        => _next.ClearAsync(cancelToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing) _next.Dispose();
        base.Dispose(disposing);
    }
}
