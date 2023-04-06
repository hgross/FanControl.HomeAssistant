# Source, Dest, Unpack folder
$url = "https://github.com/Rem0o/FanControl.Releases/raw/master/FanControl.zip"
$dl_zip_dest = ".\download\FanControl.zip"
$dl_folder = ".\download"
$temp_unzip_folder = ".\temp\FanControl"
$dll_temp_path = ".\temp\FanControl\FanControl.Plugins.dll"
$lib_folder = ".\lib"
$fc_folder = ".\devbuild\FanControl"

# ensure folders
New-Item -Force -Path $unzip_dest_folder -ItemType Directory
New-Item -Force -Path $dl_folder -ItemType Directory
New-Item -Force -Path $temp_unzip_folder -ItemType Directory
New-Item -Force -Path $fc_folder -ItemType Directory

# Download file
Invoke-WebRequest -Uri $url -OutFile $dl_zip_dest

# unzip fancontrol to temp dir
Expand-Archive -Path $dl_zip_dest -DestinationPath $temp_unzip_folder

# unzip FanControl to devbuild dir
Expand-Archive -Path $dl_zip_dest -DestinationPath $fc_folder

# copy dll to lib folder
Copy-Item $dll_temp_path -Destination $lib_folder

# cleanup
Remove-Item $dl_folder -Recurse
Remove-Item $temp_unzip_folder -Recurse
