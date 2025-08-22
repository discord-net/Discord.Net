namespace Discord.Rest.Api;

public abstract record OneOf<A, B>
{
    public sealed record Left(A Value) : OneOf<A, B>
    {
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }

    public sealed record Right(B Value) : OneOf<A, B>
    {
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}