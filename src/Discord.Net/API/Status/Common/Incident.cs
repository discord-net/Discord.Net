using Discord.API.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API.Status
{
	public class Incident
	{
		[JsonProperty("page")]
		public PageData Page { get; }
		[JsonProperty("scheduled_maintenances")]
		public MaintenanceData[] ScheduledMaintenances { get; }

        public sealed class PageData
		{
			[JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
			public ulong Id { get; }
            [JsonProperty("name")]
			public string Name { get; }
            [JsonProperty("url")]
			public string Url { get; }
            [JsonProperty("updated-at")]
			public DateTime? UpdatedAt { get; }
        }

		public sealed class MaintenanceData
		{
            //TODO: Complete
		}
	}
}
