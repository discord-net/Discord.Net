using SharpCompress.Readers;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio.Dependencies
{
    internal sealed class Ffmpeg : Dependency
    {
        private const string GithubRepositoryReleaseBaseUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest";
        private const string OsxDownloadUrl = "https://evermeet.cx/ffmpeg/getrelease/zip";

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
                downloadUrl = OsxDownloadUrl;
            }
            else
            {
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

                downloadUrl = $"{GithubRepositoryReleaseBaseUrl}-{platform}64-gpl.{fileExtension}";
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

        /// <summary>
        /// Downloads the FFmpeg binary for the current platform and extracts it to the working directory.
        /// </summary>
        /// <param name="httpClient">
        /// The <see cref="System.Net.Http.HttpClient"/> instance used to perform the HTTP request.
        /// </param>
        /// <returns>A task that represents the asynchronous download and extraction operation.</returns>
        /// <remarks>
        /// The download source depends on the current platform:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <b>macOS</b> — Downloaded from <see href="https://evermeet.cx/ffmpeg/getrelease/zip">evermeet.cx</see>,
        ///       a third-party site that provides up-to-date static FFmpeg builds for macOS as a ZIP.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <b>Windows / Linux</b> — Downloaded from
        ///       <see href="https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest">BtbN/FFmpeg-Builds</see>,
        ///       a GitHub repository that publishes automated GPL-licensed FFmpeg builds for multiple
        ///       platforms and architectures.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
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
