namespace Discord
{
	public class Region : StringEnum
	{
		public static readonly Region USWest = new Region("us-west");
		public static readonly Region USEast = new Region("us-east");
		public static readonly Region Singapore = new Region("singapore");
		public static readonly Region London = new Region("london");
		public static readonly Region Sydney = new Region("sydney");
		public static readonly Region Amsterdam = new Region("amsterdam");

		private Region(string value)
			: base(value) { }

		public static Region FromString(string value)
		{
			switch (value)
			{
				case null:
					return null;
				case "us-west":
					return Region.USWest;
				case "us-east":
					return Region.USEast;
				case "singapore":
					return Region.Singapore;
				case "london":
					return Region.London;
				case "sydney":
					return Region.Sydney;
				case "amsterdam":
					return Region.Amsterdam;
				default:
					return new Region(value);
			}
		}

		public static implicit operator Region(string value) => FromString(value);
		public static bool operator ==(Region a, Region b) => a?._value == b?._value;
		public static bool operator !=(Region a, Region b) => a?._value != b?._value;
		public override bool Equals(object obj) => (obj as Region)?._value == _value;
		public override int GetHashCode() => _value.GetHashCode();
	}
}
