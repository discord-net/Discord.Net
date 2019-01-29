namespace Discord
{
    public interface IEmote : IMentionable // TODO: Is `Mention` the correct verbage here?
    {
        string Name { get; }
    }
}
