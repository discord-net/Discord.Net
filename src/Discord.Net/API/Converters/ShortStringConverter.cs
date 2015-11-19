using Newtonsoft.Json;
using System;

namespace Discord.API.Converters
{
	/*internal class ShortStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(short);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IdConvert.ToShort((string)reader.Value);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(IdConvert.ToString((short)value));
		}
	}

	internal class NullableShortStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(short?);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IdConvert.ToNullableShort((string)reader.Value);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(IdConvert.ToString((short?)value));
		}
	}*/
}
