#!/usr/bin/env pwsh

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($null -eq (Get-Command "dotnet" -ErrorAction Ignore)) {
    throw "Could not find 'dotnet', please install .NET SDK"
}

Push-Location (Split-Path $MyInvocation.MyCommand.Definition)

try {
    & dotnet run --project build --no-launch-profile -- $args
}
finally {
    Pop-Location
}
