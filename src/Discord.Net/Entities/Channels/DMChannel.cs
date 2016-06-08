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
        public override DiscordClient Discord { get; }
        public User Recipient { get; private set; }

        public virtual IReadOnlyCollection<IMessage> CachedMessages => ImmutableArray.Create<IMessage>();

        public DMChannel(DiscordClient discord, User recipient, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Recipient = recipient;

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            Recipient.Update(model.Recipient, UpdateSource.Rest);
        }

        public async Task Update()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannel(Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task Close()
        {
            await Discord.ApiClient.DeleteChannel(Id).ConfigureAwait(false);
        }

        public virtual async Task<IUser> GetUser(ulong id)
        {
            var currentUser = await Discord.GetCurrentUser().ConfigureAwait(false);
            if (id == Recipient.Id)
                return Recipient;
            else if (id == currentUser.Id)
                return currentUser;
            else
                return null;
        }
        public virtual async Task<IReadOnlyCollection<IUser>> GetUsers()
        {
            var currentUser = await Discord.GetCurrentUser().ConfigureAwait(false);
            return ImmutableArray.Create<IUser>(currentUser, Recipient);
        }
        public virtual async Task<IReadOnlyCollection<IUser>> GetUsers(int limit, int offset)
        {
            var currentUser = await Discord.GetCurrentUser().ConfigureAwait(false);
            return new IUser[] { currentUser, Recipient }.Skip(offset).Take(limit).ToImmutableArray();
        }

        public async Task<IMessage> SendMessage(string text, bool isTTS)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateDMMessage(Id, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public async Task<IMessage> SendFile(string filePath, string text, bool isTTS)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadDMFile(Id, file, args).ConfigureAwait(false);
                return new Message(this, new User(Discord, model.Author), model);
            }
        }
        public async Task<IMessage> SendFile(Stream stream, string filename, string text, bool isTTS)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadDMFile(Id, stream, args).ConfigureAwait(false);
            return new Message(this, new User(Discord, model.Author), model);
        }
        public virtual async Task<IMessage> GetMessage(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelMessage(Id, id).ConfigureAwait(false);
            if (model != null)
                return new Message(this, new User(Discord, model.Author), model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessages(int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IMessage>> GetMessages(ulong fromMessageId, Direction dir, int limit)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, new User(Discord, x.Author), x)).ToImmutableArray();
        }
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteDMMessages(Id, new DeleteMessagesParams { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }   

        public async Task TriggerTyping()
        {
            await Discord.ApiClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }        
        
        public override string ToString() => '@' + Recipient.ToString();
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

        IUser IDMChannel.Recipient => Recipient;
        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;
    }
}
