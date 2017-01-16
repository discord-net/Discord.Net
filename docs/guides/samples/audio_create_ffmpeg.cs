private Process CreateStream(string path)
{
    var ffmpeg = new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        RedirectStandardOutput = true,
    };
    return Process.Start(ffmpeg);
}