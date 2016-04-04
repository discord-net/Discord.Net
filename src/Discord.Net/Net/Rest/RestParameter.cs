namespace Discord.Net.Rest
{
    public struct RestParameter
    {
        public string Key { get; }
        public string Value { get; }

        public RestParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public RestParameter(string key, object value)
        {
            Key = key;
            Value = value.ToString();
        }
    }
}
