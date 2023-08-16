using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public class SocketUser : SocketCacheableEntity<ulong, IUserModel>, IUser
    {
        public string? AvatarId
            => _source.AvatarHash;

        public string Discriminator
            => _source.Discriminator;

        public ushort DiscriminatorValue
            => ushort.Parse(Discriminator, NumberStyles.None, CultureInfo.InvariantCulture);

        public bool IsBot
            => _source.IsBot.GetValueOrDefault(false);

        public string Username
            => _source.Username;

        public UserProperties? PublicFlags
            => _source.PublicFlags;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        public string Mention => MentionUtils.MentionUser(Id);

        public UserStatus Status => throw new NotImplementedException();

        public IReadOnlyCollection<ClientType> ActiveClients => throw new NotImplementedException();

        public IReadOnlyCollection<IActivity> Activities => throw new NotImplementedException();

        public virtual bool IsWebhook { get => false; }

        private IUserModel _source;

        internal SocketUser(DiscordGatewayClient client, IUserModel model)
            : base(client, model.Id)
        {
            _source = model;
        }

        public Task<IDMChannel> CreateDMChannelAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128) => throw new NotImplementedException();
        public string GetDefaultAvatarUrl() => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        internal override IUserModel GetModel() => throw new NotImplementedException();
        internal override void Update(IUserModel model) => throw new NotImplementedException();
    }
}
