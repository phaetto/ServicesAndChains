@echo off

:: Start admin service
start .\Services.Executioner.exe --admin "{'AdminHost':'0.0.0.0','AdminPort':11001,'ApiKey':null}" &