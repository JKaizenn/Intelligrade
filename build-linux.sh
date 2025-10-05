#!/bin/bash

# IntelliGrade Linux AppImage/Binary Builder
# This script creates a self-contained Linux executable

set -e

echo "🔨 Building IntelliGrade for Linux..."

# Configuration
APP_NAME="IntelliGrade"
VERSION="1.0.0"
PROJECT_DIR="src/IntelliGrade.App"
OUTPUT_DIR="dist/linux"

# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
rm -rf "$PROJECT_DIR/bin/Release"

# Publish for Linux x64
echo "📦 Publishing for Linux x64..."
dotnet publish "$PROJECT_DIR/IntelliGrade.App.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=false \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "$OUTPUT_DIR"

# Make executable
chmod +x "$OUTPUT_DIR/IntelliGrade.App"

# Create desktop entry file
echo "📝 Creating desktop entry..."
cat > "$OUTPUT_DIR/intelligrade.desktop" << EOF
[Desktop Entry]
Version=1.0
Type=Application
Name=IntelliGrade
Comment=AI-Powered Assignment Grading
Exec=$PWD/$OUTPUT_DIR/IntelliGrade.App
Icon=$PWD/$OUTPUT_DIR/intelligrade
Terminal=false
Categories=Education;Development;
EOF

echo ""
echo "✅ Build complete!"
echo ""
echo "📦 Your Linux executable is ready at: $OUTPUT_DIR/IntelliGrade.App"
echo ""
echo "To run the app:"
echo "  ./$OUTPUT_DIR/IntelliGrade.App"
echo ""
echo "To install system-wide:"
echo "  sudo cp $OUTPUT_DIR/IntelliGrade.App /usr/local/bin/intelligrade"
echo "  sudo cp $OUTPUT_DIR/intelligrade.desktop /usr/share/applications/"
echo ""
echo "Or create an AppImage using appimagetool:"
echo "  https://appimage.github.io/appimagetool/"
echo ""
