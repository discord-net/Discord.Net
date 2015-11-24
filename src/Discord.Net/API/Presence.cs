//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API
{
	//Commands
	internal sealed class UpdateStatusCommand : WebSocketMessage<UpdateStatusCommand.Data>
	{
		public UpdateStatusCommand() : base(3) { }
		public class Data
		{
			[JsonProperty("idle_since")]
			public long? IdleSince;
			[JsonProperty("game_id")]
			public int? GameId;
		}
	}

	//Events
	internal sealed class TypingStartEvent
	{
		[JsonProperty("user_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long UserId;
		[JsonProperty("channel_id")]
		[JsonConverter(typeof(LongStringConverter))]
		public long ChannelId;
		[JsonProperty("timestamp")]
		public int Timestamp;
	}
	internal sealed class PresenceUpdateEvent : PresenceInfo { }
}
