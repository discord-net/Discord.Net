using System.Diagnostics;

namespace Discord.Net.Hanz;

public static class SelfLog
{
    public static void Write(string message)
    {
        try
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory, ".hanz", "global.log");
            File.AppendAllText(fullPath, $"[{DateTime.Now:O}] {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
