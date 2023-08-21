using System;
using System.Reflection;

namespace Discord.Gateway
{
	internal sealed class StructureDecoder
	{
		private readonly Type _type;
		private readonly Dictionary<string, Property> _properties;

		public StructureDecoder(Type t)
		{
			_type = t;

            _properties = t.GetProperties()
				.Select(x => new Property(x))
				.ToDictionary(x => x.Name, x => x);
		}

		public object? Decode(ref ETFPackDecoder decoder)
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

		private sealed class Property
		{
			public string Name
				=> _name;

			private readonly string _name;
			private readonly PropertyInfo _propInfo;

			public Property(PropertyInfo propInfo)
			{
				_propInfo = propInfo;
				_name = propInfo.GetCustomAttribute<ETFNameAttribute>()?.Name ?? propInfo.Name;
			}

			public void DecodeAndSet(object instance, ref ETFPackDecoder decoder)
			{
				var obj = decoder.Read();

				if (obj is string s && _propInfo.PropertyType == typeof(ulong))
					obj = ulong.Parse(s);

				_propInfo.SetValue(instance, obj);
			}
		}
	}
}

