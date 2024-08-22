namespace Discord.Rest;

public interface IRestMessageChannel<TSelf, TActor, TModel> : 
    IChannel, 
    IRestConstructable<TSelf, TActor, TModel>
{
    
}