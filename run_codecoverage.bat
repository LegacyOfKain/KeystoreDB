@echo off

rmdir /s /q CoverageReport

for /d /r . %%d in (TestResults) do @if exist "%%d" rmdir /s /q "%%d"

dotnet test --collect:"XPlat Code Coverage" 

reportgenerator -reports:"../**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"html"

pause