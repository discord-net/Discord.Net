namespace Discord.Models;

public interface IDeletable
{
    Task DeleteAsync(RequestOptions options = default);
}