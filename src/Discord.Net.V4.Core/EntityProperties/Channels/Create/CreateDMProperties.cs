namespace Discord;

public sealed class CreateDMProperties
{
    public EntityOrId<ulong, IUser> Recipient { get; set; }
}
