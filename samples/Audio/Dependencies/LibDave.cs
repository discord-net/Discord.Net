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

        /// <summary>
        /// Downloads the LibDave native binary for the current platform and extracts it to the working directory.
        /// </summary>
        /// <param name="httpClient">
        /// The <see cref="System.Net.Http.HttpClient"/> instance used to perform the HTTP request.
        /// </param>
        /// <returns>A task that represents the asynchronous download and extraction operation.</returns>
        /// <remarks>
        /// The binary is fetched as a ZIP from
        /// <see href="https://github.com/discord/libdave/releases/latest">discord/libdave</see>,
        /// Discord's official GitHub repository for their end-to-end encryption library.
        /// Only the native library file is extracted from the archive; the rest is discarded.
        /// </remarks>
        public override async Task DownloadAsync(HttpClient httpClient)
        {
            using Stream stream = await httpClient.GetStreamAsync(DownloadUrl);
            using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
            ZipArchiveEntry libDaveEntry = zip.Entries.First(entry => entry.Name == FileName);
            await libDaveEntry.ExtractToFileAsync(libDaveEntry.Name, true);
        }
    }
}
