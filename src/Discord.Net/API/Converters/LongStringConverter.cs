using System;
using Newtonsoft.Json;

namespace Discord.API.Converters
{
	public class LongStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(long);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IdConvert.ToLong((string)reader.Value);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(IdConvert.ToString((long)value));
		}
	}

	public class NullableLongStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(long?);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IdConvert.ToNullableLong((string)reader.Value);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(IdConvert.ToString((long?)value));
		}
	}
}
