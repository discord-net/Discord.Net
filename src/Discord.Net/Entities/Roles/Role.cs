using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Role;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class Role : SnowflakeEntity, IRole, IMentionable
    {
        public Guild Guild { get; }
        
        public Color Color { get; private set; }
        public bool IsHoisted { get; private set; }
        public bool IsManaged { get; private set; }
        public string Name { get; private set; }
        public GuildPermissions Permissions { get; private set; }
        public int Position { get; private set; }
        
        public bool IsEveryone => Id == Guild.Id;
        public string Mention => MentionUtils.Mention(this);
        public override DiscordRestClient Discord => Guild.Discord;

        public Role(Guild guild, Model model)
            : base(model.Id)
        {
            Guild = guild;

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Name = model.Name;
            IsHoisted = model.Hoist;
            IsManaged = model.Managed;
            Position = model.Position;
            Color = new Color(model.Color);
            Permissions = new GuildPermissions(model.Permissions);
        }

        public async Task ModifyAsync(Action<ModifyGuildRoleParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildRoleParams();
            func(args);
            var response = await Discord.ApiClient.ModifyGuildRoleAsync(Guild.Id, Id, args).ConfigureAwait(false);

            Update(response, UpdateSource.Rest);
        }
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteGuildRoleAsync(Guild.Id, Id).ConfigureAwait(false);
        }

        public Role Clone() => MemberwiseClone() as Role;
        
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        ulong IRole.GuildId => Guild.Id;
    }
}
