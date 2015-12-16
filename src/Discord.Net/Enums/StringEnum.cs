namespace Discord
{
	public abstract class StringEnum
	{
        public string Value { get; }

        protected StringEnum(string value)
		{
            Value = value;
		}

		public override string ToString() => Value;
	}
}
