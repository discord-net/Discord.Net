if (Test-Path Env:\APPVEYOR_BUILD_NUMBER) {
    $build = $env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0")
} else {
    $build = "dev"
}

dotnet test test/Discord.Net.Tests/Discord.Net.Tests.csproj -c "Release" --noBuild -p:BuildNumber="$build"
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }