Param(
    [string] $Configuration = "Debug",
    [string] $CodeDirectory = ".",
    [string] $TestsDirectory = ".",
    [string] $PackageOutputDirectory = "Build",
    [string] $Version,
    [string[]] $Package = @()
)

$tests = Get-ChildItem $TestsDirectory -Directory | where { $_.FullName -imatch "^.*\.(?:Specs|Tests|Test)$" }
$projects = Get-ChildItem $CodeDirectory -Recurse -File -Filter "project.json"

function Invoke-ExpressionExitCodeCheck([string] $command)
{
    Invoke-Expression $command

    if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

# Patch version
if ($Version) {
    Write-Host "Patching versions to ${Version}..."

    $projects | ForEach {
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

# Build

Write-Host "Restoring packages..."
Invoke-ExpressionExitCodeCheck "dotnet restore" -ErrorAction Stop

Write-Host "Building in ${Configuration}..."
Invoke-ExpressionExitCodeCheck "dotnet build ${CodeDirectory}\**\project.json -c ${Configuration}" -ErrorAction Stop


# Test

Write-Host "Running tests..."

[bool] $testsFailed = $false
$tests | ForEach {
    Invoke-Expression "dotnet test $($_.FullName) -c ${Configuration}"

    if (!$testsFailed) {
        $testsFailed = $LASTEXITCODE -and $LASTEXITCODE -ne 0
    }
}

if ($testsFailed) {
    Write-Host -BackgroundColor Red -ForegroundColor Yellow "Tests failed!"
    exit -1
} else {
    Write-Host -BackgroundColor Green -ForegroundColor White "All good!"
}


# NuGet packaging

Write-Host "Creating a nuget package in ${PackageOutputDirectory}"

$Package | ForEach {
    Invoke-ExpressionExitCodeCheck "dotnet pack ${CodeDirectory}\$($_) -c ${Configuration} -o ${PackageOutputDirectory}"
}
