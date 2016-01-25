using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Converters
{
	public class LongStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) 
            => objectType == typeof(ulong);
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => ((string)reader.Value).ToId();
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(((ulong)value).ToIdString());
	}

	public class NullableLongStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) 
            => objectType == typeof(ulong?);
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => ((string)reader.Value).ToNullableId();
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(((ulong?)value).ToIdString());
	}

    /*public class LongStringEnumerableConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable<ulong>);
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			List<ulong> result = new List<ulong>();
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
				foreach (var v in (IEnumerable<ulong>)value)
					writer.WriteValue(IdConvert.ToString(v));
				writer.WriteEndArray();
			}
		}
	}*/

    internal class LongStringArrayConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable<ulong[]>);
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var result = new List<ulong>();
			if (reader.TokenType == JsonToken.StartArray)
			{
				reader.Read();
				while (reader.TokenType != JsonToken.EndArray)
				{
					result.Add(((string)reader.Value).ToId());
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
				var a = (ulong[])value;
				for (int i = 0; i < a.Length; i++)
					writer.WriteValue(a[i].ToIdString());
				writer.WriteEndArray();
			}
		}
	}
}
