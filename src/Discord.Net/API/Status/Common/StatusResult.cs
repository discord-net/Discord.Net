using Newtonsoft.Json;
using System;

namespace Discord.API.Status
{
    public class StatusResult
    {
        public class PageData
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("updated_at")]
            public DateTime? UpdatedAt { get; set; }
        }

        public class IncidentData
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("page_id")]
            public string PageId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("shortlink")]
            public string Shortlink { get; set; }
            [JsonProperty("impact")]
            public string Impact { get; set; }

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }
            [JsonProperty("updated_at")]
            public DateTime UpdatedAt { get; set; }
            [JsonProperty("monitoring_at")]
            public DateTime? MonitoringAt { get; set; }
            [JsonProperty("resolved_at")]
            public DateTime? ResolvedAt { get; set; }
            [JsonProperty("scheduled_for")]
            public DateTime StartTime { get; set; }
            [JsonProperty("scheduled_until")]
            public DateTime EndTime { get; set; }

            [JsonProperty("incident_updates")]
            public IncidentUpdateData[] Updates { get; set; }
        }

        public class IncidentUpdateData
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("incident_id")]
            public string IncidentId { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("body")]
            public string Body { get; set; }

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }
            [JsonProperty("updated_at")]
            public DateTime? UpdatedAt { get; set; }
            [JsonProperty("display_at")]
            public DateTime? DisplayAt { get; set; }

        }

        [JsonProperty("page")]
        public PageData Page { get; set; }
        [JsonProperty("scheduled_maintenances")]
        public IncidentData[] ScheduledMaintenances { get; set; }
        [JsonProperty("incidents")]
        public IncidentData[] Incidents { get; set; }
    }
}
