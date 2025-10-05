# IntelliGrade Windows Installer Builder
# This script creates a self-contained Windows executable

Write-Host "🔨 Building IntelliGrade for Windows..." -ForegroundColor Green

# Configuration
$APP_NAME = "IntelliGrade"
$VERSION = "1.0.0"
$PROJECT_DIR = "src/IntelliGrade.App"
$OUTPUT_DIR = "dist/windows"

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
Remove-Item -Path $OUTPUT_DIR -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$PROJECT_DIR/bin/Release" -Recurse -Force -ErrorAction SilentlyContinue

# Publish for Windows x64
Write-Host "📦 Publishing for Windows x64..." -ForegroundColor Cyan
dotnet publish "$PROJECT_DIR/IntelliGrade.App.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=false `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $OUTPUT_DIR

Write-Host ""
Write-Host "✅ Build complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Your Windows executable is ready at: $OUTPUT_DIR\IntelliGrade.App.exe" -ForegroundColor White
Write-Host ""
Write-Host "To run the app:" -ForegroundColor Yellow
Write-Host "  .\$OUTPUT_DIR\IntelliGrade.App.exe" -ForegroundColor White
Write-Host ""
Write-Host "To create an installer, you can use:" -ForegroundColor Yellow
Write-Host "  - Inno Setup (https://jrsoftware.org/isinfo.php)" -ForegroundColor White
Write-Host "  - WiX Toolset (https://wixtoolset.org/)" -ForegroundColor White
Write-Host ""
