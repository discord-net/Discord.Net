using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APIMessage = Discord.API.Client.Message;

namespace Discord
{
	public enum MessageState : byte
    {
        /// <summary> Message did not originate from this session, or was successfully sent. </summary>
		Normal = 0,
        /// <summary> Message is current queued. </summary>
		Queued,
        /// <summary> Message was deleted before it was sent. </summary>
        Aborted,
        /// <summary> Message failed to be sent. </summary>
		Failed
    }

	public class Message
    {
        private readonly static Action<Message, Message> _cloner = DynamicIL.CreateCopyMethod<Message>();

        private static readonly Regex _userRegex = new Regex(@"<@[0-9]+>", RegexOptions.Compiled);
        private static readonly Regex _userNicknameRegex = new Regex(@"<@![0-9]+>", RegexOptions.Compiled);
        private static readonly Regex _channelRegex = new Regex(@"<#[0-9]+>", RegexOptions.Compiled);
        private static readonly Regex _roleRegex = new Regex(@"<@&[0-9]+>", RegexOptions.Compiled);
        private static readonly Attachment[] _initialAttachments = new Attachment[0];
        private static readonly Embed[] _initialEmbeds = new Embed[0];

        internal static string CleanUserMentions(Channel channel, string text, List<User> users = null)
        {
            ulong id;
            text = _userNicknameRegex.Replace(text, new MatchEvaluator(e =>
            {
                if (e.Value.Substring(3, e.Value.Length - 4).TryToId(out id))
                {
                    var user = channel.GetUserFast(id);
                    if (user != null)
                    {
                        if (users != null)
                            users.Add(user);
                        return '@' + user.Nickname;
                    }
                }
                return e.Value; //User not found or parse failed
            }));
            return _userRegex.Replace(text, new MatchEvaluator(e =>
            {
                if (e.Value.Substring(2, e.Value.Length - 3).TryToId(out id))
                {
                    var user = channel.GetUserFast(id);
                    if (user != null)
                    {
                        if (users != null)
                            users.Add(user);
                        return '@' + user.Name;
                    }
                }
                return e.Value; //User not found or parse failed
            }));
        }
        internal static string CleanChannelMentions(Channel channel, string text, List<Channel> channels = null)
        {
            var server = channel.Server;
            if (server == null) return text;

            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (e.Value.Substring(2, e.Value.Length - 3).TryToId(out id))
                {
                    var mentionedChannel = server.GetChannel(id);
                    if (mentionedChannel != null && mentionedChannel.Server.Id == server.Id)
                    {
                        if (channels != null)
                            channels.Add(mentionedChannel);
                        return '#' + mentionedChannel.Name;
                    }
                }
                return e.Value; //Channel not found or parse failed
            }));
        }
        internal static string CleanRoleMentions(Channel channel, string text, List<Role> roles = null)
		{
            var server = channel.Server;
            if (server == null) return text;

            return _roleRegex.Replace(text, new MatchEvaluator(e =>
			{
                ulong id;
                if (e.Value.Substring(3, e.Value.Length - 4).TryToId(out id))
                {
                    var role = server.GetRole(id);
                    if (role != null)
                    {
                        if (roles != null)
                            roles.Add(role);
                        return "@" + role.Name;
                    }
                }
                return e.Value; //Role not found or parse failed
			}));
		}
        
        //TODO: Move this somewhere
        private static string Resolve(Channel channel, string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            text = CleanUserMentions(channel, text);
            text = CleanChannelMentions(channel, text);
            text = CleanRoleMentions(channel, text);
            return text;
        }

        /*internal class ImportResolver : DefaultContractResolver
		{
			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
			{
				var property = base.CreateProperty(member, memberSerialization);
				if (member is PropertyInfo)
				{
					if (member.Name == nameof(ChannelId) || !(member as PropertyInfo).CanWrite)
						return null;

					property.Writable = true; //Handles private setters
				}
				return property;
			}
		}*/

        public class Attachment : File
		{
			/// <summary> Unique identifier for this file. </summary>
			public string Id { get; internal set; }
			/// <summary> Size, in bytes, of this file file. </summary>
			public int Size { get; internal set; }
			/// <summary> Filename of this file. </summary>
			public string Filename { get; internal set; }

			internal Attachment() { }
		}

		public class Embed
		{
			/// <summary> URL of this embed. </summary>
			public string Url { get; internal set; }
			/// <summary> Type of this embed. </summary>
			public string Type { get; internal set; }
			/// <summary> Title for this embed. </summary>
			public string Title { get; internal set; }
			/// <summary> Summary of this embed. </summary>
			public string Description { get; internal set; }
			/// <summary> Returns information about the author of this embed. </summary>
			public EmbedLink Author { get; internal set; }
			/// <summary> Returns information about the providing website of this embed. </summary>
			public EmbedLink Provider { get; internal set; }
			/// <summary> Returns the thumbnail of this embed. </summary>
			public File Thumbnail { get; internal set; }
            /// <summary> Returns the video information of this embed. </summary>
            public File Video { get; internal set; }

            internal Embed() { }
		}

		public class EmbedLink
        {
			/// <summary> URL of this embed provider. </summary>
			public string Url { get; internal set; }
			/// <summary> Name of this embed provider. </summary>
			public string Name { get; internal set; }

			internal EmbedLink() { }
		}

		public class File
		{
			/// <summary> Download url for this file. </summary>
			public string Url { get; internal set; }
			/// <summary> Preview url for this file. </summary>
			public string ProxyUrl { get; internal set; }
			/// <summary> Width of this file, if it is an image. </summary>
			public int? Width { get; internal set; }
			/// <summary> Height of this file, if it is an image. </summary>
			public int? Height { get; internal set; }

			internal File() { }
		}

        public DiscordClient Client => Channel.Client;

        /// <summary> Returns the unique identifier for this message. </summary>
        public ulong Id { get; internal set; }
        /// <summary> Returns the channel this message was sent to. </summary>
        public Channel Channel { get; }
        /// <summary> Returns the author of this message. </summary>
        public User User { get; }

        /// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
        public bool IsTTS { get; internal set; }
		/// <summary> Returns the state of this message. Only useful if UseMessageQueue is true. </summary>
		public MessageState State { get; internal set; }
		/// <summary> Returns the raw content of this message as it was received from the server. </summary>
		public string RawText { get; internal set; }
		/// <summary> Returns the content of this message with any special references such as mentions converted. </summary>
		public string Text { get; internal set; } 
		/// <summary> Returns the timestamp for when this message was sent. </summary>
		public DateTime Timestamp { get; private set; }
		/// <summary> Returns the timestamp for when this message was last edited. </summary>
		public DateTime? EditedTimestamp { get; private set; }
		/// <summary> Returns the attachments included in this message. </summary>
		public Attachment[] Attachments { get; private set; }
		/// <summary> Returns a collection of all embeded content in this message. </summary>
		public Embed[] Embeds { get; private set; }

        /// <summary> Returns a collection of all users mentioned in this message. </summary>
        public IEnumerable<User> MentionedUsers { get; internal set; }
		/// <summary> Returns a collection of all channels mentioned in this message. </summary>
		public IEnumerable<Channel> MentionedChannels { get; internal set; }
		/// <summary> Returns a collection of all roles mentioned in this message. </summary>
		public IEnumerable<Role> MentionedRoles { get; internal set; }

        internal int Nonce { get; set; }

        /// <summary> Gets the path to this object. </summary>
        internal string Path => $"{Server?.Name ?? "[Private]"}/{Id}";
        /// <summary> Returns the server containing the channel this message was sent to. </summary>
        public Server Server => Channel.Server;
        /// <summary> Returns if this message was sent from the logged-in accounts. </summary>
        public bool IsAuthor => User != null && User.Id == Client.CurrentUser?.Id;

        internal Message(ulong id, Channel channel, User user)
		{
            Id = id;
            Channel = channel;
            User = user;

			Attachments = _initialAttachments;
			Embeds = _initialEmbeds;
		}

		internal void Update(APIMessage model)
		{
			var channel = Channel;
			var server = channel.Server;
			if (model.Attachments != null)
			{
				Attachments = model.Attachments
					.Select(x => new Attachment()
					{
						Id = x.Id,
						Url = x.Url,
						ProxyUrl = x.ProxyUrl,
						Width = x.Width,
						Height = x.Height,
						Size = x.Size,
						Filename = x.Filename
					})
					.ToArray();
			}
			if (model.Embeds != null)
			{
				Embeds = model.Embeds.Select(x =>
				{
                    EmbedLink author = null, provider = null;
					File thumbnail = null, video = null;

					if (x.Author != null)
						author = new EmbedLink { Url = x.Author.Url, Name = x.Author.Name };
					if (x.Provider != null)
						provider = new EmbedLink { Url = x.Provider.Url, Name = x.Provider.Name };
					if (x.Thumbnail != null)
						thumbnail = new File { Url = x.Thumbnail.Url, ProxyUrl = x.Thumbnail.ProxyUrl, Width = x.Thumbnail.Width, Height = x.Thumbnail.Height };
                    if (x.Video != null)
                        video = new File { Url = x.Video.Url, ProxyUrl = null, Width = x.Video.Width, Height = x.Video.Height };

                    return new Embed
					{
						Url = x.Url,
						Type = x.Type,
						Title = x.Title,
						Description = x.Description,
						Author = author,
						Provider = provider,
						Thumbnail = thumbnail,
                        Video = video
					};
				}).ToArray();
			}
			
			if (model.IsTextToSpeech != null)
				IsTTS = model.IsTextToSpeech.Value;
			if (model.Timestamp != null)
				Timestamp = model.Timestamp.Value;
			if (model.EditedTimestamp != null)
				EditedTimestamp = model.EditedTimestamp;
			if (model.Mentions != null)
			{
				MentionedUsers = model.Mentions
					.Select(x => Channel.GetUserFast(x.Id))
					.Where(x => x != null)
					.ToArray();
			}

			if (model.Content != null)
			{
				string text = model.Content;
				RawText = text;
				//var mentionedUsers = new List<User>();
				var mentionedChannels = new List<Channel>();
				var mentionedRoles = new List<Role>();
				text = CleanUserMentions(Channel, text/*, mentionedUsers*/);
				if (server != null)
				{
					text = CleanChannelMentions(Channel, text, mentionedChannels);
					text = CleanRoleMentions(Channel, text, mentionedRoles);
					if (model.IsMentioningEveryone != null && model.IsMentioningEveryone.Value
						&& User != null && User.GetPermissions(channel).MentionEveryone)
						mentionedRoles.Add(Server.EveryoneRole);
				}
				Text = text;

				//MentionedUsers = mentionedUsers;
				MentionedChannels = mentionedChannels;
				MentionedRoles = mentionedRoles;
			}
        }

        public Task Edit(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var channel = Channel;

            if (text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {DiscordConfig.MaxMessageSize} characters or less.");
            
            Client.MessageQueue.QueueEdit(this, text);
            return TaskHelper.CompletedTask;
        }
        public Task Delete()
        {
            Client.MessageQueue.QueueDelete(this);
            return TaskHelper.CompletedTask;
        }

        /// <summary> Returns true if the logged-in user was mentioned. </summary>
        public bool IsMentioningMe(bool includeRoles = false)
        {
            User me = Server != null ? Server.CurrentUser : Channel.Client.PrivateUser;
            if (includeRoles)
            {
                return (MentionedUsers?.Contains(me) ?? false) ||
                       (MentionedRoles?.Any(x => me.HasRole(x)) ?? false);
            }
            else
                return MentionedUsers?.Contains(me) ?? false;
        }

        /// <summary>Resolves all mentions in a provided string to those users, channels or roles' names.</summary>
        public string Resolve(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return Resolve(Channel, text);
        }

        internal Message Clone()
        {
            var result = new Message();
            _cloner(this, result);
            return result;
        }
        private Message() { } //Used for cloning

        public override string ToString() => $"{User?.Name ?? "Unknown User"}: {RawText}";
	}
}
