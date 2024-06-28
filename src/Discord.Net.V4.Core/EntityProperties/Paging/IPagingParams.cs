namespace Discord;

public interface IPagingParams
{
    int? PageSize { get; }
    int? Total { get; }
}
