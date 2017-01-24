if (Test-Path Env:\APPVEYOR_BUILD_NUMBER) {
    $build = [convert]::ToInt32($env:APPVEYOR_BUILD_NUMBER).ToString("00000")
} else {
    $build = "dev"
}

dotnet pack "src\Discord.Net\Discord.Net.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Core\Discord.Net.Core.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Commands\Discord.Net.Commands.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Rest\Discord.Net.Rest.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.WebSocket\Discord.Net.WebSocket.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Rpc\Discord.Net.Rpc.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Providers.WS4Net\Discord.Net.Providers.WS4Net.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet pack "src\Discord.Net.Providers.UdpClient\Discord.Net.Providers.UdpClient.csproj" -c "Release" -o "../../nupkgs" --no-build -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }