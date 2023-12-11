$assembly_path_debug = ".\FanControl.HomeAssistant\bin\Debug\net48"
$assembly_path_release = ".\FanControl.HomeAssistant\bin\Release\net48"
$plugin_target_dir = ".\out\FanControl.HomeAssistant"
$fc_devbuild_target_dir = ".\out\FanControlDevBuild"
$fc_devbuild_plugin_target_dir  = "${fc_devbuild_target_dir}\Plugins"
$fc_release_input_dir = ".\devbuild\FanControl"
$release_package_target_dir = ".\out\release_package"

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


############
##  Package release
############
# Determine package name
$gitHash = (git rev-parse --short HEAD)
$gitTag = (git describe --exact-match --tags 2>&1)
if(($LASTEXITCODE -eq 0) -and $gitTag) {
    $version = $gitTag;
} else {
    $version = $gitHash;
}
$release_package_file_name = "FanControl.HomeAssistant_" + $version + ".zip"
$release_package_abspath = $release_package_target_dir + "\" + $release_package_file_name

New-Item -Force -Path $release_package_target_dir -ItemType Directory
Compress-Archive -Path $plugin_target_dir\* -CompressionLevel Fastest -DestinationPath $release_package_abspath