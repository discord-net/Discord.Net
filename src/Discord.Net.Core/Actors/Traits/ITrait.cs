namespace Discord;

public interface ITrait<out TSelf> :
    IClientProvider
    where TSelf : ITrait<TSelf>
{
    
}