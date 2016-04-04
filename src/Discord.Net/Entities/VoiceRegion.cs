using System;
using System.Threading.Tasks;
using Model = Discord.API.Rest.GetVoiceRegionsResponse;

namespace Discord
{
    public class VoiceRegion : IEntity<string>
    {
        /// <inheritdoc />
        public string Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }

        /// <summary> Gets the name of this voice region. </summary>
        public string Name { get; private set; }
        /// <summary> Returns true if this voice region is exclusive to VIP accounts. </summary>
        public bool IsVip { get; private set; }
        /// <summary> Returns true if this voice region is the closest to your machine. </summary>
        public bool IsOptimal { get; private set; }
        /// <summary> Gets an example hostname for this voice region. </summary>
        public string SampleHostname { get; private set; }
        /// <summary> Gets an example port for this voice region. </summary>
        public int SamplePort { get; private set; }

        internal VoiceRegion(string id, DiscordClient client)
        {
            Id = id;
            Discord = client;
        }

        /// <inheritdoc />
        public Task Update() { throw new NotSupportedException(); } //TODO: Not supported yet

        public void Update(Model model)
        {
            Name = model.Name;
            IsVip = model.IsVip;
            IsOptimal = model.IsOptimal;
            SampleHostname = model.SampleHostname;
            SamplePort = model.SamplePort;
        }

        public override string ToString()
        {
            string suffix = "";

            if (IsVip)
            {
                if (IsOptimal)
                    suffix = " (VIP, Optimal)";
                else
                    suffix = " (VIP)";
            }
            else if (IsOptimal)
                suffix = " (Optimal)";

            return Name + suffix;
        }
    }
}
