# FanControl.HomeAssistant
[FanControl](https://github.com/Rem0o/FanControl.Releases)-Plugin for [HomeAssistant](https://www.home-assistant.io/) sensors.

## Installation
Drop `tbd` into the Plugins Folder.
## Configuration
TODO: describe -> create auth token, put it somewhere (tbd)

# Development

## Build, package, run
From within the root directory
```
.\update-fancontrol-api.ps1
.\build.ps1
dotnet run --project TestProgram/TestProgram.csproj
```

Pick up the release build from the `out/FanControl.HomeAssistant` directory and place it within your FanControl installation's plugin folder (`FanControl/Plugins`).
Alternatively the build script builds a readily usable portable FanControl configuration within the `out/FanControlDevBuild` directory (for dev purposes).


## Tools
- `update-fancontrol-api.ps1` downloads the latest FanControl master to extract the FanControl Plugins API dll to `.\lib`
