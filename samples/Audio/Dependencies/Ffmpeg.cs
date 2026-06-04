using SharpCompress.Readers;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio.Dependencies
{
    internal sealed class Ffmpeg : Dependency
    {
        public override string Name => "Ffmpeg";

        public override string DownloadUrl { get; }

        public Ffmpeg()
        {
            DownloadUrl = GetFfmpegDownloadUrl();
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

        public override bool IsInstalled()
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

        public override async Task DownloadAsync(HttpClient httpClient)
        {
            using Stream stream = await httpClient.GetStreamAsync(DownloadUrl);

            if (DownloadUrl.EndsWith("zip"))
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
        }
    }
}
