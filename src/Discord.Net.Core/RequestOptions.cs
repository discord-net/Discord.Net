namespace Discord;

public readonly record struct RequestOptions(
    RequestFlags RequestFlags = RequestFlags.Default,
    string? AuditLogReason = null,
    CancellationToken CancellationToken = default
)
{
    public static implicit operator RequestOptions(CancellationToken token) => new(CancellationToken: token);
}