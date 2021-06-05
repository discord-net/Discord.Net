using System.Text;

namespace Discord.Net.SourceGenerators.Serialization
{
    internal static class Utils
    {
        private static readonly StringBuilder CaseChangeBuffer = new();

        public static string ConvertToSnakeCase(string value)
        {
            foreach (var c in value)
            {
                if (char.IsUpper(c) && CaseChangeBuffer.Length > 0)
                    _ = CaseChangeBuffer.Append('_');

                _ = CaseChangeBuffer.Append(char.ToLower(c));
            }

            var result = CaseChangeBuffer.ToString();
            _ = CaseChangeBuffer.Clear();

            return result;
        }
    }
}
