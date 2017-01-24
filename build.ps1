if (Test-Path Env:\APPVEYOR_BUILD_NUMBER) {
    $build = [convert]::ToInt32($env:APPVEYOR_BUILD_NUMBER).ToString("00000")
} else {
    $build = "dev"
}

dotnet restore Discord.Net.sln
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode)  }
dotnet build Discord.Net.sln -c "Release" -p:BuildNumber="$build"