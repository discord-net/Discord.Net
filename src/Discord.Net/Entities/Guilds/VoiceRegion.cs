using System.Diagnostics;
using Model = Discord.API.VoiceRegion;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class VoiceRegion : IVoiceRegion
    {
        public string Id { get; }
        public string Name { get; }
        public bool IsVip { get; }
        public bool IsOptimal { get; }
        public string SampleHostname { get; }
        public int SamplePort { get; }

        public VoiceRegion(Model model)
        {
            Id = model.Id;
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
