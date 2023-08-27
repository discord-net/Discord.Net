using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Discord.Gateway
{
	internal sealed class StructureEncoder
	{
		private readonly Type _type;
		private readonly List<Property> _props;

		public StructureEncoder(Type t)
		{
			_type = t;
			_props = t
				.GetProperties()
				.Select(x => new Property(x))
				.ToList();
		}

		public void Encode(object? value, ref ETFEncoder encoder)
		{
			if(value is null)
			{
				encoder.WriteNil();
				return;
			}

			encoder.WriteMapHeader(_props.Count);

            foreach (var prop in _props)
			{
				encoder.WriteBinary(prop.Name);
				prop.Encode(value, ref encoder);
			}
		}

		private sealed class Property
		{
			public bool IsOptional { get; }
			public Type Type { get; }
			public string Name { get; }
			public Type UnwrappedType { get; }

			private readonly PropertyInfo _propertyInfo;

			public Property(PropertyInfo info)
			{
				_propertyInfo = info;

                Name = info.GetCustomAttribute<ETFNameAttribute>()?.Name
					?? info.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
					?? info.Name;

				Type = info.PropertyType;
				IsOptional = false; // Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Optional<>);
				// if optional
				UnwrappedType = Type;
			}

			public void Encode(object instance, ref ETFEncoder encoder)
			{
				var value = _propertyInfo.GetValue(instance);

				if (value is null)
				{
                    encoder.WriteNil();
					return;
                }

				switch (value)
				{
					case ulong u:
						encoder.Write(u.ToString());
						break;
                    default:
						encoder.Write(value);
						break;
				}
			}
		} 
	}
}

