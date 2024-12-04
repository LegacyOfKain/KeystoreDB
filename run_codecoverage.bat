@echo off

dotnet test --collect:"XPlat Code Coverage" 

reportgenerator -reports:"../**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"html"

pause