namespace Discord.Models;

public interface ITrait<out TSelf> :
    IClientProvider
    where TSelf : ITrait<TSelf>
{
    
}