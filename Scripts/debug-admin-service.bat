@echo off

cd ..\Executioner

:: Start admin service
echo Starting administration server...
start .\Services.Executioner.exe --admin "{'AdminHost':'127.0.0.1','AdminPort':11001}"
:: Modules:[ { ModuleType:'Platform.ModernSoftwareDesign.Contexts.Security.MsdAuthenticationContext', ModuleParameters:['Xml', 'connection-string'], ModuleDllPath:'Platform.ModernSoftwareDesign.dll' } ]

:: Start update watcher for development admin
echo Starting update service for admin...
.\Services.Executioner.exe --start "{'Id':'Admin-Updater','ServiceName':'General', 'ContextType':'Services.Management.Administration.Update.WatchFilesystemForAdminUpdate','Parameters':['C:\\Users\\Alex-TP\\Google Drive\\Projects\\Solutions\\ServicesAndChains\\Services.Executioner\\bin\\Debug\\Services.Executioner.exe',5],'AdminHost':'localhost','AdminPort':11001}"

:: Start update watcher for admin-UI service
echo Starting update service for UI...
.\Services.Executioner.exe --start "{'Id':'UI-Updater','ServiceName':'General', 'ContextType':'Services.Management.Administration.Update.WatchFilesystemForFileCopy','Parameters':['C:\\AdminService\\UI\\', 'C:\\Users\\Alex-TP\\Google Drive\\Projects\\Solutions\\ServicesAndChains\\Services.UI\\bin\\Debug\\Services.UI.exe','C:\\AdminService\\Executioner\\Services.Executioner.exe',5],'AdminHost':'localhost','AdminPort':11001}"

echo Starting xConsole
.\Services.Executioner.exe --start "{'Id':'Admin Console','ServiceName':'xConsole','HostProcessFileName':'Services.Executioner.exe','DllPath':'Msd.Services.xConsole.dll','ContextType':'Msd.Services.xConsole.ConsoleHttpHandler, Msd.Services.xConsole, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','Parameters':['localhost',12011,'Admin console','Connected to admin.'],'AdminHost':'localhost','AdminPort':11001,'DataStructureVersionNumber':1, Modules:[ {ModuleType:'Chains.Play.Security.Provider.Simple.SingleApiKeyRequirement',ModuleParameters:['1234']}, {ModuleType:'Msd.Services.xConsole.Modules.AdministrationCommands',ModuleParameters:[]} ]}"

:: Localhost Msd services
::.\Services.Executioner.exe --start "{'Id':'Main','ServiceName':'LocalhostMsdServices','HostProcessFileName':'Services.Executioner.exe','DllPath':'Platform.ModernSoftwareDesign.dll','ContextType':'Platform.ModernSoftwareDesign.Contexts.LocalhostServices.LocalhostServicesHost, Platform.ModernSoftwareDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','AdminHost':'localhost','AdminPort':11001,'DataStructureVersionNumber':4,'Parameters':['localhost',667]}"

:: Trustpilot utilities
::.\Services.Executioner.exe --start "{'Id':'Emergency-Service','ServiceName':'RobotLoader','HostProcessFileName':'Services.Executioner.exe','DllPath':'RobotLoader.dll','ContextType':'RobotLoader.Emergency.EmergencyStarter, RobotLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','Parameters':['SearchBatchUpdate,SearchIndexing,WallE,AndrewMartin,TrackingWorker', '1,3,1,1,3'],'AdminHost':'localhost','AdminPort':11001,'ContextServerPort':0,'DataStructureVersionNumber':1}"
