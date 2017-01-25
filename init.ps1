if (Test-Path Env:\APPVEYOR_BUILD_NUMBER) {
    $env:BUILD = "rc-$($env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0"))"
} else {
    $env:BUILD = "dev"
}