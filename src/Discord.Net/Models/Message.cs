using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Message
	{
		public sealed class Attachment : File
		{
			/// <summary> Unique identifier for this file. </summary>
			public string Id { get; internal set; }
			/// <summary> Size, in bytes, of this file file. </summary>
			public int Size { get; internal set; }
			/// <summary> Filename of this file. </summary>
			public string Filename { get; internal set; }
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
		}
		public sealed class EmbedReference
		{
			/// <summary> URL of this embed provider. </summary>
			public string Url { get; internal set; }
			/// <summary> Name of this embed provider. </summary>
			public string Name { get; internal set; }
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
		}

		private readonly DiscordClient _client;
		private string _cleanText;

		/// <summary> Returns the global unique identifier for this message. </summary>
		public string Id { get; internal set; }
		/// <summary> Returns the local unique identifier for this message. </summary>
		public string Nonce { get; internal set; }

		/// <summary> Returns true if the logged-in user was mentioned. </summary>
		/// <remarks> This is not set to true if the user was mentioned with @everyone (see IsMentioningEverone). </remarks>
		public bool IsMentioningMe { get; internal set; }
		/// <summary> Returns true if @everyone was mentioned by someone with permissions to do so. </summary>
		public bool IsMentioningEveryone { get; internal set; }
		/// <summary> Returns true if the message was sent as text-to-speech by someone with permissions to do so. </summary>
		public bool IsTTS { get; internal set; }
		/// <summary> Returns true if the message is still in the outgoing message queue. </summary>
		public bool IsQueued { get; internal set; }
		/// <summary> Returns true if the message was rejected by the server. </summary>
		public bool HasFailed { get; internal set; }
		/// <summary> Returns the raw content of this message as it was received from the server.. </summary>
		public string RawText { get; internal set; }
		/// <summary> Returns the content of this message with any special references such as mentions converted. </summary>
		/// <remarks> This value is lazy loaded and only processed on first request. Each subsequent request will pull from cache. </remarks>
		public string Text => _cleanText != null ? _cleanText : (_cleanText = _client.Messages.CleanText(RawText));
		/// <summary> Returns the timestamp for when this message was sent. </summary>
		public DateTime Timestamp { get; internal set; }
		/// <summary> Returns the timestamp for when this message was last edited. </summary>
		public DateTime? EditedTimestamp { get; internal set; }
		/// <summary> Returns the attachments included in this message. </summary>
		public Attachment[] Attachments { get; internal set; }
		/// <summary> Returns a collection of all embeded content in this message. </summary>
		public Embed[] Embeds { get; internal set; }

		/// <summary> Returns a collection of all user ids mentioned in this message. </summary>
		public string[] MentionIds { get; internal set; }
		/// <summary> Returns a collection of all users mentioned in this message. </summary>
		[JsonIgnore]
		public IEnumerable<User> Mentions => MentionIds.Select(x => _client.Users[x]).Where(x => x != null);

		/// <summary> Returns the id of the server containing the channel this message was sent to. </summary>
		public string ServerId => Channel.ServerId;
		/// <summary> Returns the server containing the channel this message was sent to. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[Channel.ServerId];

		/// <summary> Returns the id of the channel this message was sent to. </summary>
		public string ChannelId { get; }
		/// <summary> Returns the channel this message was sent to. </summary>
		[JsonIgnore]
		public Channel Channel => _client.Channels[ChannelId];

		/// <summary> Returns true if the current user created this message. </summary>
		public bool IsAuthor => _client.CurrentUserId == UserId;
		/// <summary> Returns the id of the author of this message. </summary>
		public string UserId { get; internal set; }
		/// <summary> Returns the author of this message. </summary>
		[JsonIgnore]
		public User User => _client.Users[UserId];
		/// <summary> Returns the author of this message. </summary>
		[JsonIgnore]
		public Member Member => _client.Members[ServerId, UserId];

		internal Message(DiscordClient client, string id, string channelId, string userId)
		{
			_client = client;
			Id = id;
			ChannelId = channelId;
			UserId = userId;
		}

		internal void Update(API.Message model)
		{
			if (model.Attachments != null)
			{
				Attachments = model.Attachments.Select(x => new Attachment
				{
					Id = x.Id,
					Url = x.Url,
					ProxyUrl = x.ProxyUrl,
					Size = x.Size,
					Filename = x.Filename,
					Width = x.Width,
					Height = x.Height
				}).ToArray();
			}
			else
				Attachments = new Attachment[0];
			if (model.Embeds != null)
			{
				Embeds = model.Embeds.Select(x =>
				{
					var embed = new Embed
					{
						Url = x.Url,
						Type = x.Type,
						Description = x.Description,
						Title = x.Title
					};
					if (x.Provider != null)
					{
						embed.Provider = new EmbedReference
						{
							Url = x.Provider.Url,
							Name = x.Provider.Name
						};
					}
					if (x.Author != null)
					{
						embed.Author = new EmbedReference
						{
							Url = x.Author.Url,
							Name = x.Author.Name
						};
					}
					if (x.Thumbnail != null)
					{
						embed.Thumbnail = new File
						{
							Url = x.Thumbnail.Url,
							ProxyUrl = x.Thumbnail.ProxyUrl,
							Width = x.Thumbnail.Width,
							Height = x.Thumbnail.Height
						};
					}
					return embed;
				}).ToArray();
			}
			else
				Embeds = new Embed[0];
			IsMentioningEveryone = model.IsMentioningEveryone;
			IsTTS = model.IsTextToSpeech;
			MentionIds = model.Mentions?.Select(x => x.Id)?.ToArray() ?? new string[0];
			IsMentioningMe = MentionIds.Contains(_client.CurrentUserId);
			RawText = model.Content;
			_cleanText = null;
			Timestamp = model.Timestamp;
			EditedTimestamp = model.EditedTimestamp;
			if (model.Author != null)
				UserId = model.Author.Id;
		}

		public override string ToString() => User.ToString() + ": " + RawText;
	}
}
