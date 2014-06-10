
./Services.Executioner.exe --start "{'Id':'Admin-Update','ServiceName':'Update','Version':1,'HostProcessFileName':'Services.Executioner.exe','DllPath':'Services.Packages.dll','ContextType':'Services.Packages.Admin.WatchForAdminUpdate, Services.Packages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','Parameters':['update.msd.am',12345,86400],'AdminHost':'localhost','AdminPort':11001,'ContextServerPort':0,',DataStructureVersionNumber':1}"

./Services.Executioner.exe --start "{'Id':'Self-Updater','ServiceName':'Update','Version':1,'HostProcessFileName':'Services.Executioner.exe','DllPath':'Services.Packages.dll','ContextType':'Services.Packages.Admin.WatchPackagesForUpdateWithoutVersionUpdate, Services.Packages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','Parameters':['update.msd.am',12345,'Services.Update','Update',86400],'AdminHost':'localhost','AdminPort':11001,'ContextServerPort':0,'DataStructureVersionNumber':1}"

