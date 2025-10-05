#!/bin/bash

# IntelliGrade macOS App Bundle Builder
# This script creates a self-contained macOS .app bundle

set -e

echo "🔨 Building IntelliGrade for macOS..."

# Configuration
APP_NAME="IntelliGrade"
BUNDLE_ID="com.intelligrade.app"
VERSION="1.0.0"
PROJECT_DIR="src/IntelliGrade.App"
PUBLISH_DIR="$PROJECT_DIR/bin/Release/net9.0/osx-arm64/publish"
APP_BUNDLE="$APP_NAME.app"
CONTENTS_DIR="$APP_BUNDLE/Contents"
MACOS_DIR="$CONTENTS_DIR/MacOS"
RESOURCES_DIR="$CONTENTS_DIR/Resources"

# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf "$APP_BUNDLE"
rm -rf "$PROJECT_DIR/bin/Release"

# Publish the app for macOS (ARM64 - M1/M2/M3)
echo "📦 Publishing for macOS ARM64..."
dotnet publish "$PROJECT_DIR/IntelliGrade.App.csproj" \
    -c Release \
    -r osx-arm64 \
    --self-contained true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false

# Create .app bundle structure
echo "📁 Creating .app bundle structure..."
mkdir -p "$MACOS_DIR"
mkdir -p "$RESOURCES_DIR"

# Copy published files to MacOS directory
echo "📋 Copying application files..."
cp -r "$PUBLISH_DIR"/* "$MACOS_DIR/"

# Make the executable... executable
chmod +x "$MACOS_DIR/IntelliGrade.App"

# Create Info.plist
echo "📝 Creating Info.plist..."
cat > "$CONTENTS_DIR/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>CFBundleExecutable</key>
    <string>IntelliGrade.App</string>
    <key>CFBundleIconFile</key>
    <string>avalonia-logo</string>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
</dict>
</plist>
EOF

# Create PkgInfo
echo "📝 Creating PkgInfo..."
echo -n "APPL????" > "$CONTENTS_DIR/PkgInfo"

echo ""
echo "✅ Build complete!"
echo ""
echo "📦 Your macOS app is ready: $APP_BUNDLE"
echo ""
echo "To run the app:"
echo "  open $APP_BUNDLE"
echo ""
echo "To move to Applications folder:"
echo "  mv $APP_BUNDLE /Applications/"
echo ""
echo "Note: If you see a security warning, go to:"
echo "System Preferences > Security & Privacy > General"
echo "and click 'Open Anyway'"
echo ""
