
# Development
## Build, package, run
From within the root directory in a PowerShell
```powershell
# Download dependencies (once)
.\update-fancontrol-api.ps1

# Clean & Build everything (per dev cycle)
.\build.ps1

# Start pre-packaged FanControl instance.
.\out\FanControlDevBuild\FanControl.exe
```

Pick up the release build from the `out/FanControl.HomeAssistant` directory and place it within your FanControl installation's plugin folder (`FanControl/Plugins`).
Alternatively the build script builds a readily usable portable FanControl configuration within the `out/FanControlDevBuild` directory (for dev purposes).

## Tools
- `update-fancontrol-api.ps1` downloads the latest FanControl master to extract the FanControl Plugins API dll to `.\lib`
- `build.ps1`cleans, builds, packages the plugin and a pre-configured FanControl instance ready to be started. Check the `out` directory
