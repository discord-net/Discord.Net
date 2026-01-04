using Discord.LibDave;
using Discord.LibDave.Binding;
using Discord.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams;

public sealed class DaveEncryptStream : AudioOutStream
{
    private readonly DaveEncryptor _encryptor;
    private readonly AudioOutStream _next;
    private readonly Logger _logger;
    private readonly uint _ssrc;

    internal DaveEncryptStream(
        DaveEncryptor encryptor,
        AudioOutStream next,
        Logger logger,
        uint ssrc
    )
    {
        _encryptor = encryptor;
        _next = next;
        _logger = logger;
        _ssrc = ssrc;
    }

    public override void WriteHeader(ushort seq, uint timestamp, bool missed) =>
        _next.WriteHeader(seq, timestamp, missed);

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var result = _encryptor.Encrypt(
            new ReadOnlyMemory<byte>(buffer, offset, count),
            MediaType.Audio,
            _ssrc,
            out var encrypted
        );

        if (result is not EncryptorResultCode.Success)
        {
            await _logger.WarningAsync($"Failed to encrypt dave audio: {result}");
            encrypted.Dispose();
            return;
        }

        try
        {
            await _next.WriteAsync(
                encrypted.ToMemory(),
                cancellationToken
            );
        }
        finally
        {
            encrypted.Dispose();
        }
    }

    public override Task FlushAsync(CancellationToken cancelToken)
        => _next.FlushAsync(cancelToken);

    public override Task ClearAsync(CancellationToken cancelToken)
        => _next.ClearAsync(cancelToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _next.Dispose();
        base.Dispose(disposing);
    }
}
