# IntelliGrade Windows Installer Builder
# This script creates a self-contained Windows executable and installer

Write-Host "🔨 Building IntelliGrade for Windows..." -ForegroundColor Green

# Configuration
$APP_NAME = "IntelliGrade"
$VERSION = "0.9.0-beta"
$PROJECT_DIR = "src/IntelliGrade"
$OUTPUT_DIR = "dist/windows"

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
Remove-Item -Path $OUTPUT_DIR -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$PROJECT_DIR/bin/Release" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "dist/*.exe" -Force -ErrorAction SilentlyContinue

# Publish for Windows x64 (self-contained)
Write-Host "📦 Publishing for Windows x64 (self-contained)..." -ForegroundColor Cyan
dotnet publish "$PROJECT_DIR/IntelliGrade.App.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $OUTPUT_DIR

Write-Host ""
Write-Host "✅ Build complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Your Windows executable is ready at: $OUTPUT_DIR\IntelliGrade.App.exe" -ForegroundColor White
Write-Host ""

# Check if Inno Setup is installed
$InnoSetupPath = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (Test-Path $InnoSetupPath) {
    Write-Host "📀 Creating installer with Inno Setup..." -ForegroundColor Cyan

    # Run Inno Setup compiler
    & $InnoSetupPath "installer.iss"

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Installer created successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📀 Installer: dist\IntelliGrade-v$VERSION-Windows-Setup.exe" -ForegroundColor White
        Write-Host ""
        Write-Host "🚀 Ready to distribute!" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "⚠️  Installer creation failed" -ForegroundColor Red
        Write-Host ""
    }
} else {
    Write-Host "ℹ️  Inno Setup not found - skipping installer creation" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To create an installer:" -ForegroundColor Yellow
    Write-Host "  1. Install Inno Setup from: https://jrsoftware.org/isdl.php" -ForegroundColor White
    Write-Host "  2. Run this script again" -ForegroundColor White
    Write-Host ""
    Write-Host "The standalone executable is ready to use:" -ForegroundColor Yellow
    Write-Host "  .\$OUTPUT_DIR\IntelliGrade.App.exe" -ForegroundColor White
    Write-Host ""
}

Write-Host "To test the app:" -ForegroundColor Yellow
Write-Host "  .\$OUTPUT_DIR\IntelliGrade.App.exe" -ForegroundColor White
Write-Host ""
