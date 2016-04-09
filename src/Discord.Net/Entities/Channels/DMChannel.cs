using Discord.API.Rest;
using Discord.Net;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    public class DMChannel : IEntity<ulong>, IMessageChannel
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }

        /// <inheritdoc />
        public DMUser Recipient { get; private set; }

        /// <inheritdoc />
        public string Name => $"@{Recipient.Username}#{Recipient.Discriminator}";
        /// <inheritdoc />
        public IEnumerable<IUser> Users => ImmutableArray.Create<IUser>(Discord.CurrentUser, Recipient);
        /// <inheritdoc />
        ChannelType IChannel.Type => ChannelType.DM;

        private readonly MessageManager _messages;
        
        internal DMChannel(ulong id, DiscordClient client, int messageCacheSize)
        {
            Id = id;
            Discord = client;
            _messages = new MessageManager(this, messageCacheSize);
        }
        internal void Update(Model model)
        {
            if (Recipient == null)
                Recipient = Discord.CreateDMUser(this, model.Recipient);
            else
                Recipient.Update(model.Recipient);
        }

        /// <inheritdoc />
        public IUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser;
            else
                return null;
        }

        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id)
            => _messages.Get(id);
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100)
            => _messages.GetMany(limit);
        /// <inheritdoc />
        public Task<IEnumerable<Message>> GetMessages(int limit = 100, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before)
            => _messages.GetMany(limit, relativeMessageId, relativeDir);

        /// <inheritdoc />
        public Task<Message> SendMessage(string text, bool isTTS = false)
            => _messages.Send(text, isTTS);
        /// <inheritdoc />
        public Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
            => _messages.SendFile(filePath, text, isTTS);
        /// <inheritdoc />
        public Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
            => _messages.SendFile(stream, filename, text, isTTS);

        /// <inheritdoc />
        public Task TriggerTyping()
            => _messages.TriggerTyping();

        /// <inheritdoc />
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteChannelRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        /// <inheritdoc />
        public async Task Update()
        {
            var response = await Discord.RestClient.Send(new GetChannelRequest(Id)).ConfigureAwait(false);
            if (response != null)
                Update(response);
        }

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}
