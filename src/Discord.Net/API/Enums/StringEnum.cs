namespace Discord
{
	public abstract class StringEnum
	{
		protected string _value;
		protected StringEnum(string value)
		{
			_value = value;
		}

		public string Value => _value;
		public override string ToString() => _value;
	}
}
