$assembly_path_debug = ".\FanControl.HomeAssistant\bin\Debug\net48"
$assembly_path_release = ".\FanControl.HomeAssistant\bin\Release\net48"
$plugin_target_dir = ".\out\FanControl.HomeAssistant"
$fc_devbuild_target_dir = ".\out\FanControlDevBuild"
$fc_devbuild_plugin_target_dir  = "${fc_devbuild_target_dir}\Plugins"
$fc_release_input_dir = ".\devbuild\FanControl"

############
##  Clean
############
dotnet clean
Remove-Item ".\out" -Recurse -Force -ErrorAction SilentlyContinue

############
##  Build dev release
############
# debug build
dotnet build
## exclude the FanControl.Plugins.dll from buld
Remove-Item "${assembly_path_debug}\FanControl.Plugins.dll" -Recurse
## copy fc to devbuild
Copy-Item -Path $fc_release_input_dir -Destination $fc_devbuild_target_dir -Recurse
## create plugin dir in fc devbuild
New-Item -Force -Path $fc_devbuild_plugin_target_dir -ItemType Directory
## copy built artifacts into plugin dir
Copy-Item "${assembly_path_debug}\*" -Destination $fc_devbuild_plugin_target_dir -Recurse -Force

############
##  Build release
############
# release build
dotnet build -c release
## exclude the FanControl.Plugins.dll from buld
Remove-Item "${assembly_path_release}\FanControl.Plugins.dll" -Recurse
## copy built artifacts into target dir
Copy-Item $assembly_path_release -Destination $plugin_target_dir -Recurse -Force