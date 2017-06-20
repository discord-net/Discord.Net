using Newtonsoft.Json;

using Model = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class MemberRoleAuditLogChange : IAuditLogChange
    {
        internal MemberRoleAuditLogChange(BaseDiscordClient discord, Model model)
        {
            RoleAdded = model.ChangedProperty == "$add";
            RoleId = model.NewValue.Value<ulong>("id");
        }

        public bool RoleAdded { get; set; }
        //TODO: convert to IRole
        public ulong RoleId { get; set; }
    }
}
