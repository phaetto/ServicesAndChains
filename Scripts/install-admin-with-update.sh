
echo Downloading executioner...
./Services.Package.Download.exe "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'./admin/executioner/','PackageNames':['Services.Executioner']}"

echo Downloading and installing update...
./Services.Package.Download.exe "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'./admin/executioner/repository/Update/1/','PackageNames':['Services.Update']}"