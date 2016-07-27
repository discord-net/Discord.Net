using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class DMChannel : SnowflakeEntity, IDMChannel
    {
        public override DiscordRestClient Discord { get; }
        public IUser Recipient { get; private set; }

        public virtual IReadOnlyCollection<IMessage> CachedMessages => ImmutableArray.Create<IMessage>();
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

        public DMChannel(DiscordRestClient discord, IUser recipient, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Recipient = recipient;

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (/*source == UpdateSource.Rest && */IsAttached) return;
            
            (Recipient as User).Update(model.Recipients.Value[0], source);
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannelAsync(Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task CloseAsync()
        {
            await Discord.ApiClient.DeleteChannelAsync(Id).ConfigureAwait(false);
        }

        public virtual async Task<IUser> GetUserAsync(ulong id)
        {
            var currentUser = await Discord.GetCurrentUserAsync().ConfigureAwait(false);
            if (id == Recipient.Id)
                return Recipient;
            else if (id == currentUser.Id)
                return currentUser;
            else
                return null;
        }
        public virtual async Task<IReadOnlyCollection<IUser>> GetUsersAsync()
        {
            var currentUser = await Discord.GetCurrentUserAsync().ConfigureAwait(false);
            return ImmutableArray.Create(currentUser, Recipient);
        }

        public async Task<IMessage> SendMessageAsync(string text, bool isTTS)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateDMMessageAsync(Id, args).ConfigureAwait(false);
            return new Message(this, new User(model.Author.Value), model);
        }
        public async Task<IMessage> SendFileAsync(string filePath, string text, bool isTTS)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams(file) { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadDMFileAsync(Id, args).ConfigureAwait(false);
                return new Message(this, new User(model.Author.Value), model);
            }
        }
        public async Task<IMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
        {
            var args = new UploadFileParams(stream) { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadDMFileAsync(Id, args).ConfigureAwait(false);
            return new Message(this, new User(model.Author.Value), model);
        }
        public virtual async Task<IMessage> GetMessageAsync(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelMessageAsync(Id, id).ConfigureAwait(false);
            if (model != null)
                return new Message(this, new User(model.Author.Value), model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(x.Author.Value), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessagesAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(x.Author.Value), x)).ToImmutableArray();
        }
        public async Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteDMMessagesAsync(Id, new DeleteMessagesParams { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }   

        public async Task TriggerTypingAsync()
        {
            await Discord.ApiClient.TriggerTypingIndicatorAsync(Id).ConfigureAwait(false);
        }        
        
        public override string ToString() => '@' + Recipient.ToString();
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
        
        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;
    }
}
