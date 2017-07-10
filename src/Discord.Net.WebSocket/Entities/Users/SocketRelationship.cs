using Model = Discord.API.Relationship;

namespace Discord.WebSocket
{
    public class SocketRelationship : IRelationship
    {
        public RelationshipType Type { get; internal set; }

        public IUser User { get; internal set; }

        internal SocketRelationship()
        {
        }

        internal static SocketRelationship Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            SocketGlobalUser user = SocketGlobalUser.Create(discord, state, model.User);
            return new SocketRelationship
            {
                Type = model.Type,
                User = user
            };
        }
    }
}
