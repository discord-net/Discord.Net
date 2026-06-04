using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio.Dependencies
{
    internal sealed class LibDave : Dependency
    {
        private const string DownloadBaseUrl = "https://github.com/discord/libdave/releases/latest/download/libdave";

        public override string Name => "LibDave";
        public override string DownloadUrl { get; }
        private string FileName { get; }

        public LibDave()
        {
            (FileName, DownloadUrl) = GetLibDaveReleaseInfo();
        }

        private (string fileName, string downloadUrl) GetLibDaveReleaseInfo()
        {
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

            return (fileName, $"{DownloadBaseUrl}-{os}-{architecture}64-boringssl.zip");
        }

        public override bool IsInstalled()
        {
            return File.Exists(FileName);
        }

        public override async Task DownloadAsync(HttpClient httpClient)
        {
            string zipFolderPath = $"runtimes/{ZipFolderName}/native";
            using Stream stream = await httpClient.GetStreamAsync(DownloadUrl);
            using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
            ZipArchiveEntry libsodiumEntry = zip.Entries.First(entry => entry.FullName.StartsWith(zipFolderPath));
            await libsodiumEntry.ExtractToFileAsync(libsodiumEntry.Name, true);
        }
    }
}
