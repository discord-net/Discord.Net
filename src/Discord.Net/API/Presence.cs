//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

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
			public ulong? IdleSince;
			[JsonProperty("game_id")]
			public int? GameId;
		}
	}

	//Events
	internal sealed class TypingStartEvent
	{
		[JsonProperty("user_id")]
		public string UserId;
		[JsonProperty("channel_id")]
		public string ChannelId;
		[JsonProperty("timestamp")]
		public int Timestamp;
	}
	internal sealed class PresenceUpdateEvent : PresenceInfo { }
}
