namespace Discord.Rest;

public interface IRestMessageChannel : 
    IMessageChannel,
    IRestMessageChannelTrait,
    IRestEntity<ulong>;