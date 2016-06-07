using Newtonsoft.Json;
 
 namespace Discord.API.Client.Rest
 {
     [JsonObject(MemberSerialization.OptIn)]
     public class BulkMessageDelete : IRestRequest
     {
         string IRestRequest.Endpoint => $"/channels/{ChannelId}/messages/bulk_delete";
         string IRestRequest.Method => "POST";
         object IRestRequest.Payload => this;
 
         public ulong ChannelId { get; set; }
 
         public BulkMessageDelete(ulong channelId, ulong[] messageIds)
         {
             ChannelId = channelId;
             MessageIds = messageIds;
         }
 
         [JsonProperty("messages")]
         public ulong[] MessageIds { get; set; }
     }
 }