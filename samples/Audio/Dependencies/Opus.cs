using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio.Dependencies
{
    internal sealed class Opus : Dependency
    {
        private const string DownloadBaseUrl = "https://github.com/AvionBlock/OpusSharp/raw/refs/heads/master/OpusSharp.Natives/runtimes/";

        public override string Name => "Opus";
        public override string DownloadUrl { get; }
        private string FileName { get; }

        public Opus()
        {
            (FileName, DownloadUrl) = GetOpusReleaseInfo();
        }

        private (string fileName, string downloadUrl) GetOpusReleaseInfo()
        {
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

            return (fileName, $"{DownloadBaseUrl}{os}-{architecture}/native/{fileName}");
        }

        public override bool IsInstalled()
        {
            return File.Exists(FileName);
        }

        public override async Task DownloadAsync(HttpClient httpClient)
        {
            using Stream stream = await httpClient.GetStreamAsync(DownloadUrl);
            using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
            ZipArchiveEntry libDaveEntry = zip.Entries.First(entry => entry.Name == FileName);
            await libDaveEntry.ExtractToFileAsync(libDaveEntry.Name, true);
        }
    }
}
