using System;
using System.Threading.Tasks;
using Model = Discord.API.Application;

namespace Discord
{
    internal class Application : SnowflakeEntity, IApplication
    {
        protected string _iconId;
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] RPCOrigins { get; private set; }
        public ulong Flags { get; private set; }

        public override DiscordRestClient Discord { get; }
        public IUser Owner { get; private set; }

        public string IconUrl => API.CDN.GetApplicationIconUrl(Id, _iconId);

        public Application(DiscordRestClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;

            Update(model, UpdateSource.Creation);
        }

        internal void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            Description = model.Description;
            RPCOrigins = model.RPCOrigins;
            Name = model.Name;
            Flags = model.Flags;
            Owner = new User(model.Owner);
            _iconId = model.Icon;
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var response = await Discord.ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
            if (response.Id != Id)
                throw new InvalidOperationException("Unable to update this object from a different application token.");
            Update(response, UpdateSource.Rest);
        }
    }
}
