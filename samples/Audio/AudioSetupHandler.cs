using Audio.Dependencies;
using SharpCompress.Readers;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Audio;

internal class AudioSetupHandler : IDisposable
{
    private static readonly OSPlatform[] SupportedPlatforms = [OSPlatform.Linux, OSPlatform.OSX, OSPlatform.Windows];
    private static readonly Dependency[] AllDependencies = [new Ffmpeg(), new LibDave(), new Libsodium(), new Opus()];

    public async Task<bool> PrepareAsync()
    {
        if (!SupportedPlatforms.Any(RuntimeInformation.IsOSPlatform))
            throw new PlatformNotSupportedException("Unsupported operating system.");

        Dependency[] dependenciesToInstall = AllDependencies
            .Where(dependency => !dependency.IsInstalled())
            .ToArray();

        if (dependenciesToInstall.Length == 0)
            return true;

        if (!RequestUserPermission(dependenciesToInstall))
            return false;

        HttpClient httpClient = new HttpClient();
        IEnumerable<Task> installTasks = dependenciesToInstall
            .Select(dependency => dependency.DownloadAsync(httpClient));

        await Task.WhenAll(installTasks);

        return true;
    }

    private bool RequestUserPermission(IEnumerable<Dependency> dependencies)
    {
        bool? allowed = null;
        Console.WriteLine($"{binaryName} was not found. It will be downloaded from {downloadUrl}. Do you agree? (Y/N)");

        foreach (Dependency dependency in dependencies)
        {
            Console.WriteLine($"- {dependency.Name} from {dependency.DownloadUrl}");
        }

        while (allowed is null)
        {
            ConsoleKeyInfo response = Console.ReadKey();

            if (response.Key == ConsoleKey.Y)
            {
                allowed = true;
                Console.WriteLine("Downloading dependencies...");
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
