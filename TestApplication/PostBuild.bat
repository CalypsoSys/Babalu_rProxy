@echo off
set DOTNET_FRAMEWORK=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set DEBUG_CONFIG=Debug
@echo Service: %1 Using the .NET Framework located at: %DOTNET_FRAMEWORK%

@echo if [%2] == [%DEBUG_CONFIG%] copy Debug.app.config %1.config
if [%2] == [%DEBUG_CONFIG%] copy %3Debug.app.config %1.config

set DEBUG_CONFIG=
set DOTNET_FRAMEWORK=