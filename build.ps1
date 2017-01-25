if (Test-Path Env:\APPVEYOR_BUILD_NUMBER) {
    $build = $env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0")
} else {
    $build = "dev"
}

appveyor-retry dotnet restore Discord.Net.sln -v Minimal -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
dotnet build Discord.Net.sln -c "Release" -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }