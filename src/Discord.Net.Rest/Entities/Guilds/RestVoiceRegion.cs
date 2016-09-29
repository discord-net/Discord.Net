using Discord.Rest;
using System.Diagnostics;
using Model = Discord.API.VoiceRegion;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RestVoiceRegion : RestEntity<string>, IVoiceRegion
    {
        public string Name { get; private set; }
        public bool IsVip { get; private set; }
        public bool IsOptimal { get; private set; }
        public string SampleHostname { get; private set; }
        public int SamplePort { get; private set; }

        internal RestVoiceRegion(BaseDiscordClient client, string id)
            : base(client, id)
        {
        }
        internal static RestVoiceRegion Create(BaseDiscordClient client, Model model)
        {
            var entity = new RestVoiceRegion(client, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            IsVip = model.IsVip;
            IsOptimal = model.IsOptimal;
            SampleHostname = model.SampleHostname;
            SamplePort = model.SamplePort;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsVip ? ", VIP" : "")}{(IsOptimal ? ", Optimal" : "")})";
    }
}
