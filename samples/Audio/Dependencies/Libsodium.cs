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

        /// <summary>
        /// Downloads the Libsodium native binary for the current platform and extracts it to the working directory.
        /// </summary>
        /// <param name="httpClient">
        /// The <see cref="System.Net.Http.HttpClient"/> instance used to perform the HTTP request.
        /// </param>
        /// <returns>A task that represents the asynchronous download and extraction operation.</returns>
        /// <remarks>
        /// The binary is fetched from
        /// <see href="https://www.nuget.org/packages/libsodium">libsodium on NuGet</see>,
        /// the official NuGet package for the libsodium cryptography library.
        /// That package is a ZIP archive that bundle prebuilt native binaries for all platforms.
        /// </remarks>
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
