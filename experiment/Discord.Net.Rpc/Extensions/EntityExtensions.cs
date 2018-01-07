namespace Discord.Rpc
{
    internal static class EntityExtensions
    {
        public static API.Rpc.Pan ToModel(this Pan entity)
        {
            return new API.Rpc.Pan
            {
                Left = entity.Left,
                Right = entity.Right
            };
        }
        public static API.Rpc.VoiceDevice ToModel(this VoiceDevice entity)
        {
            return new API.Rpc.VoiceDevice
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
        public static API.Rpc.VoiceShortcut ToModel(this VoiceShortcut entity)
        {
            return new API.Rpc.VoiceShortcut
            {
                Code = entity.Code,
                Name = entity.Name,
                Type = entity.Type
            };
        }
    }
}
