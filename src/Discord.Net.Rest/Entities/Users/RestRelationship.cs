using System;
using System.Collections.Generic;
using System.Text;
using Model = Discord.API.Relationship;

namespace Discord.Rest
{
    public class RestRelationship : IRelationship
    {
        public RelationshipType Type { get; internal set; }

        public IUser User { get; internal set; }

        internal RestRelationship(RelationshipType type, IUser user)
        {
            Type = type;
            User = user;
        }

        internal static RestRelationship Create(BaseDiscordClient discord, Model model)
        {
            RestUser user = RestUser.Create(discord, model.User);
            return new RestRelationship(model.Type, user);
        }
    }
}
