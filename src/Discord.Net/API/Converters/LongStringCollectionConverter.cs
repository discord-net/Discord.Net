using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API.Converters
{
	public class LongStringEnumerableConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IEnumerable<long>);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			List<long> result = new List<long>();
			if (reader.TokenType == JsonToken.StartArray)
			{
				reader.Read();
				while (reader.TokenType != JsonToken.EndArray)
				{
					result.Add(IdConvert.ToLong((string)reader.Value));
					reader.Read();
				}
			}
			return result;
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
				writer.WriteNull();
			else
			{
				writer.WriteStartArray();
				foreach (var v in (IEnumerable<long>)value)
					writer.WriteValue(IdConvert.ToString(v));
				writer.WriteEndArray();
			}
		}
	}

	internal class LongStringArrayConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IEnumerable<long[]>);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			List<long> result = new List<long>();
			if (reader.TokenType == JsonToken.StartArray)
			{
				reader.Read();
				while (reader.TokenType != JsonToken.EndArray)
				{
					result.Add(IdConvert.ToLong((string)reader.Value));
					reader.Read();
                }
			}
			return result.ToArray();
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
				writer.WriteNull();
			else
			{
				writer.WriteStartArray();
				var a = (long[])value;
				for (int i = 0; i < a.Length; i++)
					writer.WriteValue(IdConvert.ToString(a[i]));
				writer.WriteEndArray();
			}
		}
	}
}
