using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Message : CachedObject<long>
	{
		public sealed class Attachment : File
		{
			/// <summary> Unique identifier for this file. </summary>
			public string Id { get; }
			/// <summary> Size, in bytes, of this file file. </summary>
			public int Size { get; }
			/// <summary> Filename of this file. </summary>
			public string Filename { get; }

			internal Attachment(string id, string url, string proxyUrl, 
				int? width, int? height, int size, string filename)
				: base(url, proxyUrl, width, height)
			{
				Id = id;
				Size = size;
				Filename = filename;
			}
		}

		public sealed class Embed
		{
			/// <summary> URL of this embed. </summary>
			public string Url { get; }
			/// <summary> Type of this embed. </summary>
			public string Type { get; }
			/// <summary> Title for this embed. </summary>
			public string Title { get; }
			/// <summary> Summary of this embed. </summary>
			public string Description { get; }
			/// <summary> Returns information about the author of this embed. </summary>
			public EmbedReference Author { get; }
			/// <summary> Returns information about the providing website of this embed. </summary>
			public EmbedReference Provider { get; }
			/// <summary> Returns the thumbnail of this embed. </summary>
			public File Thumbnail { get; }

			internal Embed(string url, string type, string title, string description, 
				EmbedReference author, EmbedReference provider, File thumbnail)
			{
				Url = url;
				Type = type;
				Title = title;
				Description = description;
				Author = author;
				Provider = provider;
				Thumbnail = thumbnail;
            }
		}

		public sealed class EmbedReference
		{
			/// <summary> URL of this embed provider. </summary>
			public string Url { get; }
			/// <summary> Name of this embed provider. </summary>
			public string Name { get; }

			internal EmbedReference(string url, string name)
			{
				Url = url;
				Name = name;
			}
		}

		public class File
		{
			/// <summary> Download url for this file. </summary>
			public string Url { get; }
			/// <summary> Preview url for this file. </summary>
			public string ProxyUrl { get; }
			/// <summary> Width of the this file, if it is an image. </summary>
			public int? Width { get; }
			/// <summary> Height of this file, if it is an image. </summary>
			public int? Height { get; }

			internal File(string url, string proxyUrl, int? width, int? height)
			{
				Url = url;
				ProxyUrl = proxyUrl;
				Width = width;
				Height = height;
			}
		}

		/// <summary> Returns true if the logged-in user was mentioned. </summary>
		/// <remarks> This is not set to true if the user was mentioned with @everyone (see IsMentioningEverone). </remarks>
		public bool IsMentioningMe { get; private set; }
		/// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
		public bool IsTTS { get; private set; }
		/// <summary> Returns true if the message is still in the outgoing message queue. </summary>
		public bool IsQueued { get; internal set; }
		/// <summary> Returns true if the message was rejected by the server. </summary>
		public bool HasFailed { get; internal set; }
		/// <summary> Returns the raw content of this message as it was received from the server.. </summary>
		public string RawText { get; private set; }
		/// <summary> Returns the content of this message with any special references such as mentions converted. </summary>
		public string Text { get; private set; } 
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

		/// <summary> Returns a collection of all channels mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> MentionedChannels { get; internal set; }

		/// <summary> Returns a collection of all roles mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<Role> MentionedRoles { get; internal set; }

		/// <summary> Returns the server containing the channel this message was sent to. </summary>
		[JsonIgnore]
		public Server Server => _channel.Value.Server;
		/// <summary> Returns the channel this message was sent to. </summary>
		[JsonIgnore]
		public Channel Channel => _channel.Value;
		private readonly Reference<Channel> _channel;

		/// <summary> Returns true if the current user created this message. </summary>
		public bool IsAuthor => _client.CurrentUserId == _user.Id;
		/// <summary> Returns the author of this message. </summary>
		[JsonIgnore]
		public User User => _user.Value;
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
					if (!channel.IsPrivate)
						return _client.Users[x, channel.Server.Id];
					else
						return _client.Users[x, null];
				});
			Attachments = _initialAttachments;
			Embeds = _initialEmbeds;
		}
		internal override void LoadReferences()
		{
			_channel.Load();
			_user.Load();
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
					.Select(x => new Attachment(x.Id, x.Url, x.ProxyUrl, x.Width, x.Height, x.Size, x.Filename))
					.ToArray();
			}
			if (model.Embeds != null)
			{
				Embeds = model.Embeds.Select(x =>
				{
					EmbedReference author = null, provider = null;
					File thumbnail = null;

					if (x.Author != null)
						author = new EmbedReference(x.Author.Url, x.Author.Name);
					if (x.Provider != null)
						provider = new EmbedReference(x.Provider.Url, x.Provider.Name);
					if (x.Thumbnail != null)
						thumbnail = new File(x.Thumbnail.Url, x.Thumbnail.ProxyUrl, x.Thumbnail.Width, x.Thumbnail.Height);

					return new Embed(x.Url, x.Type, x.Title, x.Description, author, provider, thumbnail);
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
