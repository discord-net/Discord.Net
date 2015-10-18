//Ignore unused/unassigned variable warnings
#pragma warning disable CS0649
#pragma warning disable CS0169

using Newtonsoft.Json;
using System;

namespace Discord.API
{
	public class GetIncidentsResponse
	{
		[JsonProperty("page")]
		public PageData Page;
		[JsonProperty("scheduled_maintenances")]
		public MaintenanceData[] ScheduledMaintenances;

		public sealed class PageData
		{
			[JsonProperty("id")]
			public string Id;
			[JsonProperty("name")]
			public string Name;
			[JsonProperty("url")]
			public string Url;
			[JsonProperty("updated-at")]
			public DateTime? UpdatedAt;
		}

		public sealed class MaintenanceData
		{
		}
	}
}
