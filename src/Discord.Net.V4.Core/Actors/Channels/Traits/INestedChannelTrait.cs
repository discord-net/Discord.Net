namespace Discord.Models;

public interface INestedChannelTrait
{
    IGuildChannelTrait Parent { get; }
}