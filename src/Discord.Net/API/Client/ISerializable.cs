using System.IO;

namespace Discord.API.Client
{
    public interface ISerializable
    {
        void Write(BinaryWriter writer);
    }
}
