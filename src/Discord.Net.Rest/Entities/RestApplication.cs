using System;
using System.Threading.Tasks;
using Model = Discord.API.Application;

namespace Discord.Rest
{
    public class RestApplication : RestEntity<ulong>, IApplication
    {
        protected string _iconId;
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] RPCOrigins { get; private set; }
        public ulong Flags { get; private set; }

        public IUser Owner { get; private set; }

        public string IconUrl => API.CDN.GetApplicationIconUrl(Id, _iconId);

        internal RestApplication(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestApplication Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestApplication(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {            
            Description = model.Description;
            RPCOrigins = model.RPCOrigins;
            Name = model.Name;
            Flags = model.Flags;
            Owner = RestUser.Create(Discord, model.Owner);
            _iconId = model.Icon;
        }

        public async Task UpdateAsync()
        {
            var response = await Discord.ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
            if (response.Id != Id)
                throw new InvalidOperationException("Unable to update this object from a different application token.");
            Update(response);
        }
    }
}
