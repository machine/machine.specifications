#!/usr/bin/env bash

set -euo pipefail

if which dotnet > /dev/null; then
    dotnet run --project build --no-launch-profile -- "$@"
else
    echo "error(1): Could not find 'dotnet', please install .NET SDK"
    exit 1
fi
