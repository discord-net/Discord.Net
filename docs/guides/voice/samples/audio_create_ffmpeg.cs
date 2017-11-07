private Process CreateStream(string path)
{
    return Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        RedirectStandardOutput = true,
    });
}
