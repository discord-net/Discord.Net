namespace Discord
{
	public abstract class StringEnum
	{
		private string _value;
		protected StringEnum(string value)
		{
			_value = value;
		}

		public string Value => _value;
		public override string ToString() => _value;

		public override bool Equals(object obj)
		{
			var enum2 = obj as StringEnum;
			if (enum2 == (StringEnum)null)
				return false;
			else
				return _value == enum2._value;
		}
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public static bool operator ==(StringEnum a, StringEnum b)
		{
			return a?._value == b?._value;
		}
		public static bool operator !=(StringEnum a, StringEnum b)
		{
			return a?._value != b?._value;
		}
		public static bool operator ==(StringEnum a, string b)
		{
			return a?._value == b;
		}
		public static bool operator !=(StringEnum a, string b)
		{
			return a?._value != b;
		}
		public static bool operator ==(string a, StringEnum b)
		{
			return a == b?._value;
		}
		public static bool operator !=(string a, StringEnum b)
		{
			return a != b?._value;
		}
	}
}
