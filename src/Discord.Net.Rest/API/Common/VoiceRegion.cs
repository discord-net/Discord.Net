#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class VoiceRegion
    {
        [ModelProperty("id")]
        public string Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("vip")]
        public bool IsVip { get; set; }
        [ModelProperty("optimal")]
        public bool IsOptimal { get; set; }
        [ModelProperty("sample_hostname")]
        public string SampleHostname { get; set; }
        [ModelProperty("sample_port")]
        public int SamplePort { get; set; }
    }
}
