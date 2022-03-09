using Model = Discord.API.IntegrationAccount;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IIntegrationAccount"/>.
    /// </summary>
    public class RestIntegrationAccount : IIntegrationAccount
    {
        internal RestIntegrationAccount() { }

        public string Id { get; private set; }

        public string Name { get; private set; }

        internal static RestIntegrationAccount Create(Model model)
        {
            var entity = new RestIntegrationAccount();
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            model.Name = Name;
            model.Id = Id;
        }
    }
}
