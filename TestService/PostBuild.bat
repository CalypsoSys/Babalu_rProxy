@echo off
set DOTNET_FRAMEWORK=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set DEBUG_CONFIG=Debug
@echo Service: %1 Using the .NET Framework located at: %DOTNET_FRAMEWORK%

REM Unrem the following lines the first time you build the Babalu proxy to install the service
REM %DOTNET_FRAMEWORK%\InstallUtil /u %1
REM %DOTNET_FRAMEWORK%\InstallUtil %1

@echo if [%2] == [%DEBUG_CONFIG%] copy Debug.app.config %1.config
if [%2] == [%DEBUG_CONFIG%] copy %3Debug.app.config %1.config

@echo net start "Babalu.TestService"
REM net start "Babalu.TestService"

set DEBUG_CONFIG=
set DOTNET_FRAMEWORK=