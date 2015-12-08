using Discord.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemberInfo = System.Reflection.MemberInfo;

namespace Discord
{
	public enum MessageState : byte
	{
		Normal = 0,
		Queued,
		Failed
	}

	public sealed class Message : CachedObject<long>
	{
		internal class ImportResolver : DefaultContractResolver
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
		}

		public sealed class Attachment : File
		{
			/// <summary> Unique identifier for this file. </summary>
			public string Id { get; internal set; }
			/// <summary> Size, in bytes, of this file file. </summary>
			public int Size { get; internal set; }
			/// <summary> Filename of this file. </summary>
			public string Filename { get; internal set; }

			internal Attachment() { }
		}

		public sealed class Embed
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
			public EmbedReference Author { get; internal set; }
			/// <summary> Returns information about the providing website of this embed. </summary>
			public EmbedReference Provider { get; internal set; }
			/// <summary> Returns the thumbnail of this embed. </summary>
			public File Thumbnail { get; internal set; }

			internal Embed() { }
		}

		public sealed class EmbedReference
		{
			/// <summary> URL of this embed provider. </summary>
			public string Url { get; internal set; }
			/// <summary> Name of this embed provider. </summary>
			public string Name { get; internal set; }

			internal EmbedReference() { }
		}

		public class File
		{
			/// <summary> Download url for this file. </summary>
			public string Url { get; internal set; }
			/// <summary> Preview url for this file. </summary>
			public string ProxyUrl { get; internal set; }
			/// <summary> Width of the this file, if it is an image. </summary>
			public int? Width { get; internal set; }
			/// <summary> Height of this file, if it is an image. </summary>
			public int? Height { get; internal set; }

			internal File() { }
		}

		/// <summary> Returns true if the logged-in user was mentioned. </summary>
		public bool IsMentioningMe { get; private set; }
		/// <summary> Returns true if the current user created this message. </summary>
		public bool IsAuthor => _client.CurrentUser.Id == _user.Id;
		/// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
		public bool IsTTS { get; private set; }
		/// <summary> Returns the state of this message. Only useful if UseMessageQueue is true. </summary>
		public MessageState State { get; internal set; }
		/// <summary> Returns the raw content of this message as it was received from the server. </summary>
		public string RawText { get; private set; }
		[JsonIgnore]
		/// <summary> Returns the content of this message with any special references such as mentions converted. </summary>
		public string Text { get; internal set; } 
		/// <summary> Returns the timestamp for when this message was sent. </summary>
		public DateTime Timestamp { get; private set; }
		/// <summary> Returns the timestamp for when this message was last edited. </summary>
		public DateTime? EditedTimestamp { get; private set; }
		/// <summary> Returns the attachments included in this message. </summary>
		public Attachment[] Attachments { get; private set; }
		private static readonly Attachment[] _initialAttachments = new Attachment[0];
		/// <summary> Returns a collection of all embeded content in this message. </summary>
		public Embed[] Embeds { get; private set; }
		private static readonly Embed[] _initialEmbeds = new Embed[0];
		
		/// <summary> Returns a collection of all users mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<User> MentionedUsers { get; internal set; }
		[JsonProperty]
		private IEnumerable<long> MentionedUserIds
		{
			get { return MentionedUsers?.Select(x => x.Id); }
			set { MentionedUsers = value.Select(x => _client.GetUser(Server, x)).Where(x => x != null); }
		}

		/// <summary> Returns a collection of all channels mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> MentionedChannels { get; internal set; }
		[JsonProperty]
		private IEnumerable<long> MentionedChannelIds
		{
			get { return MentionedChannels?.Select(x => x.Id); }
			set { MentionedChannels = value.Select(x => _client.GetChannel(x)).Where(x => x != null); }
		}

		/// <summary> Returns a collection of all roles mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<Role> MentionedRoles { get; internal set; }
		[JsonProperty]
		private IEnumerable<long> MentionedRoleIds
		{
			get { return MentionedRoles?.Select(x => x.Id); }
			set { MentionedRoles = value.Select(x => _client.GetRole(x)).Where(x => x != null); }
		}

		/// <summary> Returns the server containing the channel this message was sent to. </summary>
		[JsonIgnore]
		public Server Server => _channel.Value.Server;

		/// <summary> Returns the channel this message was sent to. </summary>
		[JsonIgnore]
		public Channel Channel => _channel.Value;
		[JsonProperty]
		private long? ChannelId => _channel.Id;
		private readonly Reference<Channel> _channel;

		/// <summary> Returns the author of this message. </summary>
		[JsonIgnore]
		public User User => _user.Value;
		[JsonProperty]
		private long? UserId => _user.Id;
		private readonly Reference<User> _user;

		internal Message(DiscordClient client, long id, long channelId, long userId)
			: base(client, id)
		{
			_channel = new Reference<Channel>(channelId,
				x => _client.Channels[x],
				x => x.AddMessage(this),
				x => x.RemoveMessage(this));
			_user = new Reference<User>(userId,
				x =>
				{
					var channel = Channel;
					if (channel == null) return null;

					if (!channel.IsPrivate)
						return _client.Users[x, channel.Server.Id];
					else
						return _client.Users[x, null];
				});
			Attachments = _initialAttachments;
			Embeds = _initialEmbeds;
		}
		internal override bool LoadReferences()
		{
			return _channel.Load() && _user.Load();
		}
		internal override void UnloadReferences()
		{
			_channel.Unload();
			_user.Unload();
		}

		internal void Update(MessageInfo model)
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
					EmbedReference author = null, provider = null;
					File thumbnail = null;

					if (x.Author != null)
						author = new EmbedReference { Url = x.Author.Url, Name = x.Author.Name };
					if (x.Provider != null)
						provider = new EmbedReference { Url = x.Provider.Url, Name = x.Provider.Name };
					if (x.Thumbnail != null)
						thumbnail = new File { Url = x.Thumbnail.Url, ProxyUrl = x.Thumbnail.ProxyUrl, Width = x.Thumbnail.Width, Height = x.Thumbnail.Height };

					return new Embed
					{
						Url = x.Url,
						Type = x.Type,
						Title = x.Title,
						Description = x.Description,
						Author = author,
						Provider = provider,
						Thumbnail = thumbnail
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
					.Select(x => _client.Users[x.Id, Channel.Server?.Id])
					.Where(x => x != null)
					.ToArray();
			}
			if (model.IsMentioningEveryone != null)
			{
				if (model.IsMentioningEveryone.Value && User.GetPermissions(channel).MentionEveryone)
					MentionedRoles = new Role[] { Server.EveryoneRole };
				else
					MentionedRoles = new Role[0];
            }
			if (model.Content != null)
			{
				string text = model.Content;
				RawText = text;

				//var mentionedUsers = new List<User>();
				var mentionedChannels = new List<Channel>();
				//var mentionedRoles = new List<Role>();
				text = Mention.CleanUserMentions(_client, server, text/*, mentionedUsers*/);
				if (server != null)
				{
					text = Mention.CleanChannelMentions(_client, server, text, mentionedChannels);
					//text = Mention.CleanRoleMentions(_client, User, channel, text, mentionedRoles);
				}
				Text = text;

				//MentionedUsers = mentionedUsers;
				MentionedChannels = mentionedChannels;
				//MentionedRoles = mentionedRoles;
			}

			if (server != null)
			{
				var me = server.CurrentUser;
				IsMentioningMe = (MentionedUsers?.Contains(me) ?? false) || 
					(MentionedRoles?.Any(x => me.HasRole(x)) ?? false);
			}
			else
			{
				var me = _client.PrivateUser;
				IsMentioningMe = MentionedUsers?.Contains(me) ?? false;
            }
        }

		public override bool Equals(object obj) => obj is Message && (obj as Message).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 9979);
		public override string ToString() => $"{User}: {RawText}";
	}
}
