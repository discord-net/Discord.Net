using Model = Discord.API.VoiceRegion;

namespace Discord.Rest
{
    public class VoiceRegion : IVoiceRegion
    {
        /// <inheritdoc />
        public string Id { get; }
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public bool IsVip { get; }
        /// <inheritdoc />
        public bool IsOptimal { get; }
        /// <inheritdoc />
        public string SampleHostname { get; }
        /// <inheritdoc />
        public int SamplePort { get; }

        internal VoiceRegion(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            IsVip = model.IsVip;
            IsOptimal = model.IsOptimal;
            SampleHostname = model.SampleHostname;
            SamplePort = model.SamplePort;
        }

        public override string ToString() => $"{Name ?? Id.ToString()}";
    }
}
