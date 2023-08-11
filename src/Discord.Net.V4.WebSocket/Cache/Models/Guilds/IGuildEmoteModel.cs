using System;
namespace Discord.WebSocket.Cache
{
    public interface IGuildEmoteModel : IEntityModel<ulong>
    {
        string Name { get; }
        ulong[] Roles { get; }
        bool RequireColons { get; }
        bool IsManaged { get; }
        bool IsAnimated { get; }
        bool IsAvailable { get; }

        ulong CreatorId { get; }
    }
}

