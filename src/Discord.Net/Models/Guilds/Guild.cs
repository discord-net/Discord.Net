using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Entities;
using Model = Wumpus.Entities.Guild;

namespace Discord
{
    internal class Guild : SnowflakeEntity, IGuild
    {
        public Guild(Model model, IDiscordClient discord) : base(discord)
        {
            Name = model.Name.ToString();
            AFKTimeout = model.AfkTimeout;
            IsEmbeddable = model.EmbedEnabled.GetValueOrDefault(false);

            // FYI: when casting these, make sure Wumpus is using the same schema as us, otherwise these values will not match up.
            DefaultMessageNotifications = (DefaultMessageNotifications)model.DefaultMessageNotifications;
            MfaLevel = (MfaLevel)model.MfaLevel;
            VerificationLevel = (VerificationLevel)model.VerificationLevel;
            ExplicitContentFilter = (ExplicitContentFilterLevel)model.ExplicitContentFilter;

            IconId = model.Icon?.Hash.ToString();
            IconUrl = null; // TODO: port CDN
            SplashId = model.Splash?.Hash.ToString();
            SplashUrl = null; // TODO: port CDN

            Available = model is GatewayGuild;

            AFKChannelId = model.AfkChannelId;
            EmbedChannelId = model.EmbedChannelId.IsSpecified ? model.EmbedChannelId.Value : null;
            SystemChannelId = model.SystemChannelId;

            OwnerId = model.OwnerId;
            ApplicationId = model.ApplicationId;
            VoiceRegionId = null; // TODO?

            Role[] roles = new Role[model.Roles.Length];
            Role role;
            for (int i = 0; i < model.Roles.Length; i++)
            {
                role = new Role(model.Roles[i], this, Discord);
                if (role.Id == Id) // EveryoneRole has the same ID as the guild
                    EveryoneRole = role;
                roles[i] = role;
            }
            Roles = roles;

            // TODO: emotes
            string[] features = new string[model.Features.Length];
            for (int i = 0; i < model.Features.Length; i++)
                features[i] = model.Features[i].ToString();
            Features = features;
        }

        public string Name { get; set; }
        public int AFKTimeout { get; set; }
        public bool IsEmbeddable { get; set; }

        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
        public MfaLevel MfaLevel { get; set; }
        public VerificationLevel VerificationLevel { get; set; }
        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }

        public string IconId { get; set; }
        public string IconUrl { get; set; }
        public string SplashId { get; set; }
        public string SplashUrl { get; set; }

        public bool Available { get; set; }

        public ulong? AFKChannelId { get; set; }
        public ulong? EmbedChannelId { get; set; }
        public ulong? SystemChannelId { get; set; }

        public ulong OwnerId { get; set; }
        public ulong? ApplicationId { get; set; }
        public string VoiceRegionId { get; set; }

        public IRole EveryoneRole { get; set; }
        public IReadOnlyCollection<IGuildEmote> Emotes { get; set; }
        public IReadOnlyCollection<string> Features { get; set; }
        public IReadOnlyCollection<IRole> Roles { get; set; }

        public Task DeleteAsync()
            => Discord.Rest.DeleteGuildAsync(Id);
    }
}
