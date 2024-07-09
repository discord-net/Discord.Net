using System.Diagnostics;

namespace Discord.Net.Hanz;

public static class SelfLog
{
    private const string FileName = "Hanz.log";

    public static void Write(string message)
    {
        try
        {
            var fullPath = Path.Combine(Path.GetTempPath(), FileName);
            File.AppendAllText(fullPath, $"[{DateTime.Now:O}] {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
