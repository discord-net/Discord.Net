namespace Discord.Models;

public interface IPartial<in T>
{
    void ApplyTo(T model);
}