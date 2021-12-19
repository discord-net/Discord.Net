using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class UserStatusConverter : JsonConverter
    {
        public static readonly UserStatusConverter Instance = new UserStatusConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (string)reader.Value switch
            {
                "online" => UserStatus.Online,
                "idle" => UserStatus.Idle,
                "dnd" => UserStatus.DoNotDisturb,
                "invisible" => UserStatus.Invisible,//Should never happen
                "offline" => UserStatus.Offline,
                _ => throw new JsonSerializationException("Unknown user status"),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((UserStatus)value)
            {
                case UserStatus.Online:
                    writer.WriteValue("online");
                    break;
                case UserStatus.Idle:
                case UserStatus.AFK:
                    writer.WriteValue("idle");
                    break;
                case UserStatus.DoNotDisturb:
                    writer.WriteValue("dnd");
                    break;
                case UserStatus.Invisible:
                    writer.WriteValue("invisible");
                    break;
                case UserStatus.Offline:
                    writer.WriteValue("offline");
                    break;
                default:
                    throw new JsonSerializationException("Invalid user status");
            }
        }
    }
}
