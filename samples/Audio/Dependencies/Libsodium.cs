using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio.Dependencies
{
    internal sealed class Libsodium : Dependency
    {
        private const string DownloadBaseUrl = "https://www.nuget.org/api/v2/package/libsodium";

        public override string Name => "Libsodium";
        public override string DownloadUrl => DownloadBaseUrl;
        private string ZipFolderName { get; }
        private string FileName { get; }

        public Libsodium()
        {
            (ZipFolderName, FileName) = GetLibsodiumFileInfo();
        }

        private (string zipFolderName, string fileName) GetLibsodiumFileInfo()
        {
            string fileName;
            string os;
            string architecture;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = "libsodium.so";
                os = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileName = "libsodium.dylib";
                os = "osx";
            }
            else
            {
                fileName = "libsodium.dll";
                os = "win";
            }

            if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                architecture = "arm64";
            else
                architecture = "x64";

            return ($"{os}-{architecture}", fileName);
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
