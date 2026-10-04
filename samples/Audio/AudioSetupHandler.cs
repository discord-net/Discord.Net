using Audio.Dependencies;
using System.Runtime.InteropServices;

namespace Audio;

internal class AudioSetupHandler
{
    private static readonly OSPlatform[] SupportedPlatforms = [OSPlatform.Linux, OSPlatform.OSX, OSPlatform.Windows];
    private static readonly Dependency[] AllDependencies = [new Ffmpeg(), new LibDave(), new Libsodium(), new Opus()];

    /// <summary>
    /// Prepares the environment by verifying platform compatibility and installing any missing dependencies.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if all dependencies
    /// are already installed or were successfully downloaded; <c>false</c> if the user denied permission
    /// to install the missing dependencies.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when the current operating system is not among the supported platforms.
    /// </exception>
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

        Console.WriteLine("Downloading dependencies...");

        using HttpClient httpClient = new HttpClient();
        IEnumerable<Task> installTasks = dependenciesToInstall
            .Select(dependency => dependency.DownloadAsync(httpClient));

        await Task.WhenAll(installTasks);

        Console.WriteLine("All dependencies downloaded");

        return true;
    }

    private bool RequestUserPermission(Dependency[] dependencies)
    {
        bool? allowed = null;
        Console.WriteLine($"The following {dependencies.Length} external dependencies need to be installed:");

        foreach (Dependency dependency in dependencies)
        {
            Console.WriteLine($"  - {dependency.Name} from {dependency.DownloadUrl}");
        }

        Console.WriteLine("Do you agree? (Y/N)");

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
}
