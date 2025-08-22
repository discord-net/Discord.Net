namespace Discord;

public interface IDeletable
{
    Task DeleteAsync(RequestOptions options = default);
}