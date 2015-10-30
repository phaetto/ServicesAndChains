@echo off

echo Updating developer project...
IF EXIST "MicroServicesStarter\bin\release\MicroServicesStarter.exe" (
	MicroServicesStarter\bin\release\MicroServicesStarter.exe --update
)