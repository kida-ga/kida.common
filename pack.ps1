param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Version = "0.1.0"
)

$ErrorActionPreference = "Stop"
$commonRoot = $PSScriptRoot
$packageRoot = Join-Path $commonRoot ".artifacts\packages"
$nugetConfig = Join-Path $commonRoot "NuGet.Public.config"
$commonProperties = @(
    "-p:KidaPackageVersion=$Version",
    "-p:KidaUsePackageReferences=true",
    "-p:KidaUseHaleyPackageReferences=true",
    "-p:GeneratePackageOnBuild=false"
)

New-Item -ItemType Directory -Force -Path $packageRoot | Out-Null

function Invoke-DotNet {
    param([Parameter(Mandatory)][string[]] $Arguments)

    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed with exit code $LASTEXITCODE."
    }
}

function Publish-Package {
    param([Parameter(Mandatory)][string] $Project)

    $projectPath = Join-Path $commonRoot $Project
    Invoke-DotNet (@("restore", $projectPath, "--configfile", $nugetConfig) + $commonProperties)
    Invoke-DotNet (@("build", $projectPath, "-c", "Release", "--no-restore") + $commonProperties)
    Invoke-DotNet (@(
        "pack", $projectPath,
        "-c", "Release",
        "--no-build",
        "--no-restore",
        "-o", $packageRoot,
        "-p:BuildProjectReferences=false"
    ) + $commonProperties)
}

Publish-Package "Contracts\Kida.Contracts\Kida.Contracts.csproj"
Publish-Package "Clients\Kida.Service.Client\Kida.Service.Client.csproj"
Publish-Package "ResourceServer\Kida.ResourceServer\Kida.ResourceServer.csproj"
Publish-Package "AuthEdge\Kida.AuthEdge\Kida.AuthEdge.csproj"

Write-Host "Kida public packages created in $packageRoot"
