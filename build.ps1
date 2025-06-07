# Health Tracker Build Script
param (
    [Parameter(Position = 0)]
    [string]$Target = "Build",
    
    [switch]$DetailedOutput
)

$ErrorActionPreference = "Stop"
$VerbosePreference = if ($DetailedOutput) { "Continue" } else { "SilentlyContinue" }

# Solution paths
$SolutionPath = Join-Path $PSScriptRoot "HealthTracker.sln"
$ApiProject = Join-Path $PSScriptRoot "src\HealthTracker.Api\HealthTracker.Api.csproj"
$CliProject = Join-Path $PSScriptRoot "src\HealthTracker.Cli\HealthTracker.Cli.csproj"
$DockerComposePath = Join-Path $PSScriptRoot "docker-compose.yml"
$NuGetOutputPath = Join-Path $PSScriptRoot "nupkg"

# Docker settings
$DockerContainerName = "health-tracker-api"

# Function to write colored output
function Write-ColorOutput {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [string]$ForegroundColor = "White"
    )
    
    Write-Host $Message -ForegroundColor $ForegroundColor
}

function Invoke-Clean {
    Write-ColorOutput "🧹 Cleaning solution..." -ForegroundColor Cyan
    dotnet clean $SolutionPath --verbosity minimal
    
    if (Test-Path $NuGetOutputPath) {
        Remove-Item -Path $NuGetOutputPath -Recurse -Force
    }
    
    Write-ColorOutput "✅ Clean completed" -ForegroundColor Green
}

function Invoke-Build {
    Write-ColorOutput "🔨 Building solution..." -ForegroundColor Cyan
    dotnet build $SolutionPath --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Build failed" -ForegroundColor Red
        exit 1
    }
    Write-ColorOutput "✅ Build completed" -ForegroundColor Green
}

function Invoke-Test {
    Write-ColorOutput "🧪 Running tests..." -ForegroundColor Cyan
    dotnet test $SolutionPath --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Tests failed" -ForegroundColor Red
        exit 1
    }
    Write-ColorOutput "✅ Tests completed" -ForegroundColor Green
}

function Invoke-Pack {
    Write-ColorOutput "📦 Packaging CLI..." -ForegroundColor Cyan
    
    if (-not (Test-Path $NuGetOutputPath)) {
        New-Item -Path $NuGetOutputPath -ItemType Directory | Out-Null
    }
    
    dotnet pack $CliProject --configuration Release --output $NuGetOutputPath --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Packaging failed" -ForegroundColor Red
        exit 1
    }
    Write-ColorOutput "✅ Packaging completed. NuGet package created in $NuGetOutputPath" -ForegroundColor Green
}

function Invoke-Install {
    Write-ColorOutput "📲 Installing CLI tool..." -ForegroundColor Cyan
    
    # Uninstall first to make sure we're using the latest version
    Invoke-Uninstall -Silent
    
    $packagePath = Get-ChildItem -Path $NuGetOutputPath -Filter "*.nupkg" | Select-Object -First 1
    if (-not $packagePath) {
        Write-ColorOutput "❌ No NuGet package found. Run Pack first." -ForegroundColor Red
        exit 1
    }
    
    dotnet tool install --global --add-source $NuGetOutputPath healthcli
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Installation failed" -ForegroundColor Red
        exit 1
    }
    Write-ColorOutput "✅ CLI tool installed. You can now run 'health' commands." -ForegroundColor Green
}

function Invoke-Uninstall {
    param (
        [switch]$Silent
    )
    
    if (-not $Silent) {
        Write-ColorOutput "🗑️ Uninstalling CLI tool..." -ForegroundColor Cyan
    }
    
    dotnet tool uninstall --global healthcli 2>$null
    
    if (-not $Silent) {
        Write-ColorOutput "✅ CLI tool uninstalled" -ForegroundColor Green
    }
}

function Invoke-Docker {
    Write-ColorOutput "🐳 Building Docker image..." -ForegroundColor Cyan
    docker-compose -f $DockerComposePath build
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Docker build failed" -ForegroundColor Red
        exit 1
    }
    Write-ColorOutput "✅ Docker image built" -ForegroundColor Green
}

function Invoke-RunApi {
    Write-ColorOutput "🚀 Starting API container..." -ForegroundColor Cyan
    
    # Stop existing container if running
    Invoke-StopApi -Silent
    
    docker-compose -f $DockerComposePath up -d
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Failed to start API container" -ForegroundColor Red
        exit 1
    }
    
    Write-ColorOutput "⏳ Waiting for API to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 3
    
    Write-ColorOutput "✅ API container started. Available at http://localhost:50000" -ForegroundColor Green
}

function Invoke-StopApi {
    param (
        [switch]$Silent
    )
    
    if (-not $Silent) {
        Write-ColorOutput "🛑 Stopping API container..." -ForegroundColor Cyan
    }
    
    docker-compose -f $DockerComposePath down
    
    if (-not $Silent) {
        Write-ColorOutput "✅ API container stopped" -ForegroundColor Green
    }
}

# Main execution
try {
    switch ($Target) {
        "Clean" { Invoke-Clean }
        "Build" { Invoke-Build }
        "Test" { 
            Invoke-Build
            Invoke-Test 
        }
        "Pack" { 
            Invoke-Build
            Invoke-Pack 
        }
        "Install" { 
            Invoke-Build
            Invoke-Pack
            Invoke-Install 
        }
        "Uninstall" { Invoke-Uninstall }
        "Docker" { 
            Invoke-Build
            Invoke-Docker 
        }
        "RunApi" { 
            Invoke-Docker
            Invoke-RunApi 
        }
        "StopApi" { Invoke-StopApi }
        "All" {
            Invoke-Clean
            Invoke-Build
            Invoke-Test
            Invoke-Pack
            Invoke-Docker
        }
        default {
            Write-ColorOutput "❌ Unknown target: $Target" -ForegroundColor Red
            Write-ColorOutput "Available targets: Clean, Build, Test, Pack, Install, Uninstall, Docker, RunApi, StopApi, All" -ForegroundColor Yellow
            exit 1
        }
    }
}
catch {
    Write-ColorOutput "❌ Error: $_" -ForegroundColor Red
    Write-ColorOutput $_.ScriptStackTrace -ForegroundColor Red
    exit 1
}
