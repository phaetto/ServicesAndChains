
echo Downloading executioner...
./Services.Package.Download.exe "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'./admin/executioner/','PackageNames':['Services.Executioner']}"

echo Downloading and installing update...
./Services.Package.Download.exe "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'./admin/executioner/repository/Update/1/','PackageNames':['Services.Update']}"

rm ./admin/executioner/services.packages.database.dbxml
rm ./admin/executioner/repository/Update/1/services.packages.database.dbxml

chmod +x ./admin/executioner/*

chmod +x ./admin/executioner/repository/Update/1/*