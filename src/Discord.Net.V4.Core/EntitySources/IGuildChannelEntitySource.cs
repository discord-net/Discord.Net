using Discord.EntityRelationships;

namespace Discord;

public interface IGuildChannelEntitySource : IClientProvider, ILoadableEntity<ulong, IGuildChannel>, IGuildRelationship, IPathable
{

}
