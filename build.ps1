Param(
    [switch] $AppVeyor,
    [string] $Version = "0.10.0-unstable-1",
    [string] $Configuration = "Debug"
)

function Invoke-ExpressionExitCodeCheck([string] $command)
{
    Invoke-Expression $command

    if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

[string] $packagesDirectory = "packages-dotnet";
[string] $testProjectsDirectory = "Source"
[string] $codeProjectsDirectory = "Source"
[string] $nugetOutputDirectory = "Build"

# Patch version

if ($AppVeyor) {
    Write-Host "Patching versions to ${version}..."

    Get-ChildItem $codeProjectsDirectory -Recurse -File -Filter "project.json" | ForEach {
        Write-Host "Updating version: $($_.FullName)"

        $foundVersion = $false; # replace only the first occurance of versin (assumes package version is on top)

        (Get-Content $_.FullName -ErrorAction Stop) |
            Foreach-Object {
                $versionLine = '"version":\s*?".*?"'
                if (($foundVersion -ne $true) -and ($_ -match $versionLine)) {
                    $foundVersion = $true;
                    return $_ -replace '"version":\s*?".*?"',"""version"": ""$Version"""
                } else {
                    return $_
                }
            } |
            Set-Content $_.FullName -ErrorAction Stop
    }
}

# Restore Packages
Write-Host "Restoring packages..."

# we restore in a specific folder, because we need to reference the nunit and mspec console tools
Invoke-ExpressionExitCodeCheck "dotnet restore --packages ${packagesDirectory}"

# we restore properly (otherwise seems to cause issues)
Invoke-ExpressionExitCodeCheck "dotnet restore"


[string] $netMSpecRunnerExe = $(Get-Item "${packagesDirectory}\Machine.Specifications.Runner.Console\*\tools\mspec-clr4.exe" -ErrorAction Stop).FullName

# Build
Write-Host "Building ${configuration}..."

Invoke-ExpressionExitCodeCheck "dotnet build ${codeProjectsDirectory}\Machine.Specifications -c ${configuration}" -ErrorAction Stop


# Run Mspec tests
Write-Host "Running specs..."

if ($AppVeyor) {
    $netMSpecRunnerExe += " --appveyor"
}

[bool] $specsFailed = $false

Get-ChildItem $testProjectsDirectory -Directory -Filter "*.Specs" | ForEach {
    Invoke-ExpressionExitCodeCheck "dotnet build $($_.FullName) -c ${configuration}"

    Get-Item "$($_.FullName)\bin\${configuration}\*\*.Specs.dll" -ErrorAction Stop | ForEach {
        if (!$_.FullName.Contains("netcoreapp")) {

            Invoke-Expression "${netMSpecRunnerExe} $($_.FullName)"

            if (!$specsFailed) {
                $specsFailed = $LASTEXITCODE -and $LASTEXITCODE -ne 0
            }
        }
    }
}

# Run NUnit tests
Write-Host "Running nunit tests..."

[bool] $testsFailed = $false

Get-ChildItem $testProjectsDirectory -Directory -Filter "*.Tests" -ErrorAction Stop | ForEach {
    Invoke-Expression "dotnet test $($_.FullName)"

    if (!$testsFailed) {
        $testsFailed = $LASTEXITCODE -and $LASTEXITCODE -ne 0
    }
}

if ($testsFailed -or $specsFailed) {
    Write-Host -BackgroundColor Red -ForegroundColor Yellow "Tests failed!"
    exit -1
} else {
    Write-Host -BackgroundColor Green -ForegroundColor White "All good!"
}

if ($AppVeyor) {
    # Pack NuGet
    Write-Host "Creating a nuget package in ${nugetOutputDirectory}"

    Invoke-ExpressionExitCodeCheck "dotnet pack ${codeProjectsDirectory}\Machine.Specifications -c ${configuration} -o ${nugetOutputDirectory} --version-suffix ${version}"
}