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

        /// <summary>
        /// Downloads the Opus native binary for the current platform directly to the working directory.
        /// </summary>
        /// <param name="httpClient">
        /// The <see cref="System.Net.Http.HttpClient"/> instance used to perform the HTTP request.
        /// </param>
        /// <returns>A task that represents the asynchronous download operation.</returns>
        /// <remarks>
        /// Opus is distributed as a single native library file (no archive extraction needed).
        /// The binary is fetched directly from
        /// <see href="https://github.com/AvionBlock/OpusSharp/tree/master/OpusSharp.Natives/runtimes">AvionBlock/OpusSharp</see>,
        /// a GitHub repository that hosts prebuilt Opus native libraries for multiple platforms and architectures.
        /// </remarks>
        public override async Task DownloadAsync(HttpClient httpClient)
        {
            using Stream stream = await httpClient.GetStreamAsync(DownloadUrl);
            using FileStream file = File.Create(FileName);
            await stream.CopyToAsync(file);
        }
    }
}
