# Set the project directory
$projectDir = "D:\Programming\.NET\KeystoreDB"

# Set the project name and package name
$projectName = "KeystoreDB"
$packageName = "KeystoreDB"

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

# Get and update the version
$projectFile = "$projectDir\$projectName\$projectName.csproj"

# Check if the file exists
if (Test-Path $projectFile) {
    # Read the content of the project file
    $xmlContent = Get-Content $projectFile -Raw
    
    # Parse the XML content
    $xml = [xml]$xmlContent
    
    # Find the Version element across all PropertyGroups
    $versionElement = $xml.Project.PropertyGroup | 
                      Where-Object { $_.Version } | 
                      Select-Object -First 1

    if ($versionElement) {
        $currentVersion = [Version]$versionElement.Version
        Write-Host "Current version: $currentVersion"

        # Increment the minor version
		# $newVersion = New-Object Version $currentVersion.Major, ($currentVersion.Minor + 1), 0
		$currentVersionParts = $versionElement.Version.Split('.')
        $currentPatch = [int]$currentVersionParts[2]

        Write-Host "Current Patch: $currentPatch"

        # Increment the patch version
        $newPatchVersion = $currentPatch + 1
		Write-Host "New Patch version: $newPatchVersion"

        $newVersion = New-Object Version $currentVersion.Major, $currentVersion.Minor, $newPatchVersion
		Write-Host "New version: $newVersion"

        # Update the Version element
        $versionElement.Version = $newVersion.ToString()

        # Save the changes back to the file
        $xml.Save($projectFile)

        Write-Host "Version updated to: $newVersion"
    } else {
        Write-Host "Version element not found in any PropertyGroup."
        
        # Display all PropertyGroups for debugging
        Write-Host "All PropertyGroups:"
        $xml.Project.PropertyGroup | ForEach-Object {
            Write-Host ($_.OuterXml)
        }
    }
} else {
    Write-Host "Project file not found at path: $projectFile"
}

# Create the NuGet package
Write-Host "Creating NuGet package "
dotnet pack "$projectDir\$projectName\$projectName.csproj" -c Release -o $outputDir 

# Get the created package file
$packageFile = Get-ChildItem -Path $outputDir -Filter "$packageName.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

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