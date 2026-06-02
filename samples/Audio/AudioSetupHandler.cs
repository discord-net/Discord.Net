using SharpCompress.Readers;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio;

internal class AudioSetupHandler : IDisposable
{
    private static readonly OSPlatform[] SupportedPlatforms = [OSPlatform.Linux, OSPlatform.OSX, OSPlatform.Windows];

    /*

    private const string SodiumFileName = "libsodium.dll";
    private const string OpusDownloadUrl = "https://github.com/discord-net/Discord.Net/raw/refs/heads/dev/voice-natives/vnext_natives_win32_x64.zip";
    */

    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<bool> PrepareAsync()
    {
        if (!SupportedPlatforms.Any(RuntimeInformation.IsOSPlatform))
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }

        return await DownloadFfmpegIfNotExists()
            && await DownloadLibDaveIfNotExists()
            && await DownloadOpusIfNotExists()
            && await DownloadLibsodiumIfNotExists();
    }

    private async Task<bool> DownloadFfmpegIfNotExists()
    {
        if (CheckFfmpegInstalled())
            return true;

        string downloadUrl = GetFfmpegDownloadUrl();

        if (!RequestUserPermission("Ffmpeg", downloadUrl))
            return false;

        using Stream stream = await _httpClient.GetStreamAsync(downloadUrl);

        if (downloadUrl.EndsWith("zip"))
        {
            using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
            ZipArchiveEntry ffmpegEntry = zip.Entries.First(entry => Path.GetFileNameWithoutExtension(entry.Name) == "ffmpeg");
            await ffmpegEntry.ExtractToFileAsync(ffmpegEntry.Name, true);
        }
        else
        {
            await using IAsyncReader reader = await ReaderFactory.OpenAsyncReader(stream);

            while (await reader.MoveToNextEntryAsync())
            {
                if (reader.Entry.Key is not null && reader.Entry.Key.EndsWith("bin/ffmpeg"))
                {
                    string fileName = Path.GetFileName(reader.Entry.Key)!;
                    using FileStream outputStream = File.Create(fileName);
                    await reader.WriteEntryToAsync(outputStream);
                    break;
                }
            }
        }

        return true;
    }

    private bool CheckFfmpegInstalled()
    {
        bool installed = false;

        try
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                Arguments = "-version",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process? process = Process.Start(processInfo);

            if (process is not null)
            {
                process.WaitForExit();
                installed = process.ExitCode == 0;
            }
        }
        catch (Exception) { }

        return installed;
    }

    private string GetFfmpegDownloadUrl()
    {
        string downloadUrl;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            downloadUrl = "https://evermeet.cx/ffmpeg/getrelease/zip";
        }
        else
        {
            const string baseUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest";
            string platform;
            string fileExtension;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "linux";
                fileExtension = "tar.xz";
            }
            else
            {
                platform = "win";
                fileExtension = "zip";
            }

            if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                platform += "arm";

            downloadUrl = $"{baseUrl}-{platform}64-gpl.{fileExtension}";
        }

        return downloadUrl;
    }

    private async Task<bool> DownloadLibDaveIfNotExists()
    {
        (string filename, string downloadUrl) = GetLibDaveReleaseInfo();

        if (File.Exists(filename))
            return true;

        if (!RequestUserPermission("LibDave", downloadUrl))
            return false;

        using Stream stream = await _httpClient.GetStreamAsync(downloadUrl);
        using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
        ZipArchiveEntry libDaveEntry = zip.Entries.First(entry => entry.Name == filename);
        await libDaveEntry.ExtractToFileAsync(libDaveEntry.Name, true);

        return true;
    }

    private (string filename, string downloadUrl) GetLibDaveReleaseInfo()
    {
        const string baseUrl = "https://github.com/discord/libdave/releases/latest/download/libdave";
        string os;
        string architecture;
        string fileName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            os = "Linux";
            fileName = "libdave.so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            os = "macOS";
            fileName = "libdave.dylib";
        }
        else
        {
            os = "Windows";
            fileName = "libdave.dll";
        }

        if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
            architecture = "ARM";
        else
            architecture = "X";

        return (fileName, $"{baseUrl}-{os}-{architecture}64-boringssl.zip");
    }

    private async Task<bool> DownloadOpusIfNotExists()
    {
        (string filename, string downloadUrl) = GetOpusReleaseInfo();

        if (File.Exists(filename))
            return true;

        if (!RequestUserPermission("Opus", downloadUrl))
            return false;

        using Stream stream = await _httpClient.GetStreamAsync(downloadUrl);
        using FileStream file = File.Create(filename);
        await stream.CopyToAsync(file);

        return true;
    }    

    private (string fileName, string downloadUrl) GetOpusReleaseInfo()
    {
        const string baseUrl = "https://github.com/AvionBlock/OpusSharp/raw/refs/heads/master/OpusSharp.Natives/runtimes/";
        string fileName;
        string os;
        string architecture;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            os = "linux";
            fileName = "opus.so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            os = "osx";
            fileName = "opus.dylib";
        }
        else
        {
            os = "win";
            fileName = "opus.dll";
        }

        if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
            architecture = "arm64";
        else
            architecture = "x64";

        return (fileName, $"{baseUrl}{os}-{architecture}/native/{fileName}");
    }

    private async Task<bool> DownloadLibsodiumIfNotExists()
    {
        (string filename, string downloadUrl) = GetLibsodiumReleaseInfo();

        if (File.Exists(filename))
            return true;

        if (!RequestUserPermission("Libsodium", downloadUrl))
            return false;

        using Stream stream = await _httpClient.GetStreamAsync(downloadUrl);
        using FileStream file = File.Create(filename);
        await stream.CopyToAsync(file);

        return true;
    }

    private (string fileName, string downloadUrl) GetLibsodiumReleaseInfo()
    {
        const string baseUrl = "https://github.com/AvionBlock/OpusSharp/raw/refs/heads/master/OpusSharp.Natives/runtimes/";
        string fileName;
        string os;
        string architecture;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            os = "linux";
            fileName = "opus.so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            os = "osx";
            fileName = "opus.dylib";
        }
        else
        {
            os = "win";
            fileName = "opus.dll";
        }

        if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
            architecture = "arm64";
        else
            architecture = "x64";

        return (fileName, $"{baseUrl}{os}-{architecture}/native/{fileName}");
    }

    private bool RequestUserPermission(string binaryName, string downloadUrl)
    {
        bool? allowed = null;
        Console.WriteLine($"{binaryName} was not found. It will be downloaded from {downloadUrl}. Do you agree? (Y/N)");

        while (allowed is null)
        {
            ConsoleKeyInfo response = Console.ReadKey();

            if (response.Key == ConsoleKey.Y)
            {
                allowed = true;
                Console.WriteLine();
            }
            else if (response.Key == ConsoleKey.N)
            {
                allowed = false;
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("\nInvalid input. Please press 'Y' for Yes or 'N' for No.");
            }
        }

        return allowed.Value;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
