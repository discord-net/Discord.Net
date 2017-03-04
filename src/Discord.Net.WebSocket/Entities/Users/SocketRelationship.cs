using System;

namespace Discord.WebSocket
{
    public class SocketRelationship : IRelationship
    {
        public RelationshipType Type { get; private set; }

        public IUser User { get; private set; }
    }
}
