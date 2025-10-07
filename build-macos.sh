#!/bin/bash
set -e

echo "🔨 Building IntelliGrade for macOS..."

APP_NAME="IntelliGrade"
PROJECT_DIR="src/IntelliGrade.App"
PUBLISH_DIR="$PROJECT_DIR/bin/Release/net9.0/osx-arm64/publish"
APP_BUNDLE="$APP_NAME.app"

cd "$(dirname "$0")"

echo "🧹 Cleaning previous builds..."
rm -rf "$APP_BUNDLE"
rm -rf "$PROJECT_DIR/bin/Release"

echo "📦 Publishing for macOS ARM64..."
dotnet publish "$PROJECT_DIR/IntelliGrade.App.csproj" \
    -c Release \
    -r osx-arm64 \
    --self-contained true \
    -p:PublishSingleFile=false

echo "📁 Creating .app bundle..."
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

echo "📋 Copying files..."
cp -r "$PUBLISH_DIR"/* "$APP_BUNDLE/Contents/MacOS/"
chmod +x "$APP_BUNDLE/Contents/MacOS/IntelliGrade.App"

echo "📝 Creating Info.plist..."
cat > "$APP_BUNDLE/Contents/Info.plist" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>IntelliGrade</string>
    <key>CFBundleDisplayName</key>
    <string>IntelliGrade</string>
    <key>CFBundleIdentifier</key>
    <string>com.intelligrade.app</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>IntelliGrade.App</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

echo -n "APPL????" > "$APP_BUNDLE/Contents/PkgInfo"

echo ""
echo "✅ Build complete!"
echo "📦 App ready: $APP_BUNDLE"
echo ""
echo "To run: open $APP_BUNDLE"
echo "To install: mv $APP_BUNDLE /Applications/"
