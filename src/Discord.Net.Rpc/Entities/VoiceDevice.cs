using Model = Discord.API.Rpc.VoiceDevice;

namespace Discord.Rpc
{
    public struct VoiceDevice
    {
        public string Id { get; }
        public string Name { get; }

        internal VoiceDevice(string id, string name)
        {
            Id = id;
            Name = name;
        }
        internal static VoiceDevice Create(Model model)
        {
            return new VoiceDevice(model.Id, model.Name);
        }
    }
}
