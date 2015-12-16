using Discord.API.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API.Status
{
	public class Incident
	{
		[JsonProperty("page")]
		public PageData Page { get; set; }
		[JsonProperty("scheduled_maintenances")]
		public MaintenanceData[] ScheduledMaintenances { get; set; }

        public sealed class PageData
		{
			[JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
			public ulong Id { get; set; }
            [JsonProperty("name")]
			public string Name { get; set; }
            [JsonProperty("url")]
			public string Url { get; set; }
            [JsonProperty("updated-at")]
			public DateTime? UpdatedAt { get; set; }
        }

		public sealed class MaintenanceData
		{
            //TODO: Complete
		}
	}
}
