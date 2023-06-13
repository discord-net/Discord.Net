namespace Discord.Models
{
    public interface IEmojiModel
    {
        ulong? Id { get; }
        string Name { get; }
        ulong[] Roles { get; }
        bool RequireColons { get; }
        bool IsManaged { get; }
        bool IsAnimated { get; }
        bool IsAvailable { get; }

        ulong? CreatorId { get; }
    }
}
