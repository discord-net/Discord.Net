using Model = Discord.API.Relationship;

namespace Discord.WebSocket
{
    public class SocketRelationship : IRelationship
    {
        public RelationshipType Type { get; internal set; }

        public IUser User { get; internal set; }

        public SocketRelationship(RelationshipType type, IUser user)
        {
            Type = type;
            User = user;
        }

        internal static SocketRelationship Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            SocketSimpleUser user = SocketSimpleUser.Create(discord, state, model.User);
            return new SocketRelationship(model.Type, user);
        }
    }
}
