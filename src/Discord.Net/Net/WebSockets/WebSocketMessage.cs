using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.Net.WebSockets
{
	public class WebSocketMessage
	{
		[JsonProperty(PropertyName = "op")]
		public int Operation;
		[JsonProperty(PropertyName = "d")]
		public object Payload;
		[JsonProperty(PropertyName = "t", NullValueHandling = NullValueHandling.Ignore)]
		public string Type;
		[JsonProperty(PropertyName = "s", NullValueHandling = NullValueHandling.Ignore)]
		public int? Sequence;
	}
	internal abstract class WebSocketMessage<T> : WebSocketMessage
		where T : new()
	{
		public WebSocketMessage() { Payload = new T(); }
		public WebSocketMessage(int op) { Operation = op; Payload = new T(); }
		public WebSocketMessage(int op, T payload) { Operation = op; Payload = payload; }

		[JsonIgnore]
		public new T Payload
		{
			get { if (base.Payload is JToken) { base.Payload = (base.Payload as JToken).ToObject<T>(); } return (T)base.Payload; }
			set { base.Payload = value; }
		}
	}
}
