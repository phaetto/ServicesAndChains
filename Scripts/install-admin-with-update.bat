@echo off

:: When in cmd this fails; because of the cmd base path
::echo Updating tools...
::Services.Package.Download --update

echo Download service wrapper...
Services.Package.Download "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'.\\\\admin\\\\','PackageNames':['Services.Windows.Initiator']}"

echo Downloading executioner...
Services.Package.Download "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'.\\\\admin\\\\executioner\\\\','PackageNames':['Services.Executioner']}"

echo Downloading and installing update...
Services.Package.Download "{'UpdateServerHostname':'update.msd.am','UpdateServerPort': 12345,'PackageFolder':'.\\\\admin\\\\executioner\\\\repository\\\\Update\\\\1\\\\','PackageNames':['Services.Update']}"

xcopy service-start-with-update.bat admin\start.bat /-Y /R

echo Installing service...
admin\install.bat

net start "Admin Initiator"