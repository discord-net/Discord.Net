using System;
using System.Numerics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Discord.Gateway
{
	internal sealed class StructureDecoder
	{
		public Dictionary<string, Property> Properties
			=> _properties;

		private readonly Type _type;
		private readonly Dictionary<string, Property> _properties;

		public StructureDecoder(Type t)
		{
			_type = t;

            _properties = t.GetProperties()
				.Select(x => new Property(x))
				.ToDictionary(x => x.Name, x => x);
		}

		public object? Decode(ref ETFDecoder decoder)
		{
			var length = decoder.ReadInt();

			var inst = Activator.CreateInstance(_type)!;

            for (int i = 0; i != length; i++)
			{
				var rawKey = decoder.Read();

				if (rawKey is not string name)
					throw new FormatException($"Expected string for keyname, but got {rawKey?.GetType().Name ?? "null"}");

				if (_properties.TryGetValue(name, out var prop))
					prop.DecodeAndSet(inst, ref decoder);
			}

			return inst;
		}

		public sealed class Property
		{
			public string Name
				=> _name;

			private readonly string _name;
			private readonly PropertyInfo _propInfo;

			public Property(PropertyInfo propInfo)
			{
				_propInfo = propInfo;
                _name = propInfo.GetCustomAttribute<ETFNameAttribute>()?.Name
                    ?? propInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                    ?? propInfo.Name;
            }

			public void ConvertAndSet(object instance, object? obj)
			{
                if (obj is string s && _propInfo.PropertyType == typeof(ulong))
                    obj = ulong.Parse(s);

                if (obj is not null && obj.GetType() != _propInfo.PropertyType && !obj.GetType().IsAssignableTo(_propInfo.PropertyType))
                {
                    var targetType = _propInfo.PropertyType;

                    if (targetType.IsEnum)
                    {
                        obj = Enum.ToObject(targetType, obj);
                        goto setvalue;
                    }

                    if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        targetType = targetType.GetGenericArguments()[0];
                    }

                    obj = Convert.ChangeType(obj, targetType);
                }

            setvalue:

                _propInfo.SetValue(instance, obj);
            }

			public void DecodeAndSet(object instance, ref ETFDecoder decoder)
			{
                var obj = decoder.Read();
				ConvertAndSet(instance, obj);
            }
		}
	}
}

