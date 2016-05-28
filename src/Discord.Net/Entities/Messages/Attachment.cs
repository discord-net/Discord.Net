using Model = Discord.API.Attachment;

namespace Discord
{
    public struct Attachment
    {
        public ulong Id { get; }
        public int Size { get; }
        public string Filename { get; }

        public Attachment(Model model)
        {
            Id = model.Id;
            Size = model.Size;
            Filename = model.Filename;
        }
    }
}
