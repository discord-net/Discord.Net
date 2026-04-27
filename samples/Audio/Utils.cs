using System.IO.Compression;

namespace Audio;

internal static class Utils
{
    public const string FfmpegFileName = "ffmpeg.exe";
    private const string FfmpegDownloadUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
    private const string LibDaveFileName = "libdave.dll";
    private const string LibDaveDownloadUrl = "https://github.com/discord/libdave/releases/latest/download/libdave-Windows-X64-boringssl.zip";
    private const string OpusFileName = "opus.dll";
    private const string SodiumFileName = "libsodium.dll";
    private const string OpusDownloadUrl = "https://github.com/discord-net/Discord.Net/raw/refs/heads/dev/voice-natives/vnext_natives_win32_x64.zip";

    public static Task DownloadBinariesAsync()
    {
        return Task.WhenAll(
            DownloadFfmpegIfNotExists(),
            DownloadLibDaveIfNotExists(),
            DownloadOpusIfNotExists());
    }

    private static async Task DownloadFfmpegIfNotExists()
    {
        if (File.Exists(FfmpegFileName))
            return;

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(FfmpegDownloadUrl);
        using Stream bodyStream = await response.Content.ReadAsStreamAsync();
        using ZipArchive zip = new ZipArchive(bodyStream, ZipArchiveMode.Read);
        ZipArchiveEntry ffmpegEntry = zip.Entries.First(entry => entry.Name == FfmpegFileName);
        await ffmpegEntry.ExtractToFileAsync(FfmpegFileName, true);
    }

    private static async Task DownloadLibDaveIfNotExists()
    {
        if (File.Exists(LibDaveFileName))
            return;

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(LibDaveDownloadUrl);
        using Stream bodyStream = await response.Content.ReadAsStreamAsync();
        using ZipArchive zip = new ZipArchive(bodyStream, ZipArchiveMode.Read);
        ZipArchiveEntry libDaveEntry = zip.GetEntry($"bin/{LibDaveFileName}")!;
        await libDaveEntry.ExtractToFileAsync(LibDaveFileName, true);
    }

    private static async Task DownloadOpusIfNotExists()
    {
        if (File.Exists(OpusFileName) && File.Exists(SodiumFileName))
            return;

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(OpusDownloadUrl);
        using Stream bodyStream = await response.Content.ReadAsStreamAsync();
        using ZipArchive zip = new ZipArchive(bodyStream, ZipArchiveMode.Read);

        if (!File.Exists(OpusFileName))
        {
            ZipArchiveEntry opusEntry = zip.Entries.First(entry => entry.Name.EndsWith(OpusFileName));
            await opusEntry.ExtractToFileAsync(OpusFileName, true);
        }

        if (!File.Exists(SodiumFileName))
        {
            ZipArchiveEntry sodiumEntry = zip.GetEntry(SodiumFileName)!;
            await sodiumEntry.ExtractToFileAsync(SodiumFileName, true);
        }
    }
}
