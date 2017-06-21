using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class ChannelDeleteChanges : IAuditLogChanges
    {
        private ChannelDeleteChanges(string name, ChannelType type, IReadOnlyCollection<Overwrite> overwrites)
        {
            ChannelName = name;
            ChannelType = type;
            Overwrites = overwrites;
        }

        internal static ChannelDeleteChanges Create(BaseDiscordClient discord, ChangeModel[] models)
        {
            //Use FirstOrDefault here in case the order ever changes
            var overwritesModel = models.FirstOrDefault(x => x.ChangedProperty == "permission_overwrites");
            var typeModel = models.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = models.FirstOrDefault(x => x.ChangedProperty == "name");

            var overwrites = overwritesModel.OldValue.ToObject<API.Overwrite[]>()
                .Select(x => new Overwrite(x.TargetId, x.TargetType, new OverwritePermissions(x.Allow, x.Deny)))
                .ToList();
            var type = typeModel.OldValue.ToObject<ChannelType>();
            var name = nameModel.OldValue.ToObject<string>();

            return new ChannelDeleteChanges(name, type, overwrites.ToReadOnlyCollection());
        }

        public string ChannelName { get; }
        public ChannelType ChannelType { get; }
        public IReadOnlyCollection<Overwrite> Overwrites { get; }
    }
}
