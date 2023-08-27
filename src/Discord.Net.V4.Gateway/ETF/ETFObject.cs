using System;
namespace Discord.Gateway
{
	public sealed class ETFObject
	{
		private readonly Dictionary<object, object?> _dict;

		public ETFObject(Dictionary<object, object?> dict)
		{
			_dict = dict;
		}

		public T ToObject<T>()
		{
			var instance = Activator.CreateInstance<T>() ?? throw new NullReferenceException("Null instance after reflective construction");

            var decoder = ETF.GetOrCreateDecoder(typeof(T));

			foreach(var kvp in _dict)
			{
				if (kvp.Key is string str && decoder.Properties.TryGetValue(str, out var property))
					property.ConvertAndSet(instance, kvp.Value);
			}

			return instance;
		}
	}
}