# Set the project directory
$projectDir = "D:\Programming\.NET\KeystoreDB"

# Set the project name
$projectName = "KeystoreDB.Infrastructure"

# Set the output directory for the NuGet package
$outputDir = "$projectDir\nupkg"

# Ensure the output directory exists
if (!(Test-Path -Path $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir
}

# Remove existing NuGet packages
Write-Host "Removing existing NuGet packages..."
Remove-Item -Path "$outputDir\*.nupkg" -Force

# Build the project in Release configuration
Write-Host "Building the project..."
dotnet build "$projectDir\$projectName\$projectName.csproj" -c Release

# Get the current version from the project file
$projectFile = "$projectDir\$projectName\$projectName.csproj"
$xml = [xml](Get-Content $projectFile)
$currentVersion = $xml.Project.PropertyGroup.Version

# Increment the version
$versionParts = $currentVersion -split '\.'
$newVersion = "{0}.{1}.{2}" -f $versionParts[0], $versionParts[1], ([int]$versionParts[2] + 1)

# Update the version in the project file
$xml.Project.PropertyGroup.Version = $newVersion
$xml.Save($projectFile)

# Create the NuGet package with the new version
Write-Host "Creating NuGet package with version $newVersion..."
dotnet pack "$projectDir\$projectName\$projectName.csproj" -c Release -o $outputDir

# Get the created package file
$packageFile = Get-ChildItem -Path $outputDir -Filter "*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if ($packageFile) {
    Write-Host "NuGet package created successfully: $($packageFile.FullName)"

    # Check for NuGet API key in environment variables
    $nugetApiKey = $env:NUGET_API_KEY

    if ($nugetApiKey) {
        # Push the package to NuGet
        Write-Host "Pushing package to NuGet..."
        dotnet nuget push $packageFile.FullName --api-key $nugetApiKey --source https://api.nuget.org/v3/index.json

        Write-Host "Package pushed to NuGet successfully."
    } else {
        Write-Host "NuGet API key not found in environment variables. Package not pushed."
    }
} else {
    Write-Host "Failed to create NuGet package."
}