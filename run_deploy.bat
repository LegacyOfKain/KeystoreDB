@echo off
echo Setting NuGet API Key...
set /p NUGET_API_KEY="Enter your NuGet API Key: "
setx NUGET_API_KEY "%NUGET_API_KEY%"

echo Running deployment script...
powershell -ExecutionPolicy Bypass -File "%~dp0deploy.ps1"

pause