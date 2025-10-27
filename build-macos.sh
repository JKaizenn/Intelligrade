#!/bin/bash
set -e

echo "🔨 Building IntelliGrade for macOS..."

APP_NAME="IntelliGrade"
VERSION="0.9.0-beta"
PROJECT_DIR="src/IntelliGrade.App"
PUBLISH_DIR="$PROJECT_DIR/bin/Release/net9.0/osx-arm64/publish"
APP_BUNDLE="$APP_NAME.app"
DMG_NAME="IntelliGrade-v${VERSION}-macOS.dmg"
DMG_TEMP="dmg_temp"

cd "$(dirname "$0")"

echo "🧹 Cleaning previous builds..."
rm -rf "$APP_BUNDLE"
rm -rf "$DMG_TEMP"
rm -f "$DMG_NAME"
rm -rf "$PROJECT_DIR/bin/Release"

echo "📦 Publishing for macOS ARM64 (self-contained)..."
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

echo "🎨 Copying app icon..."
cp "$PROJECT_DIR/Assets/IntelliGrade.icns" "$APP_BUNDLE/Contents/Resources/"

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
    <string>0.9.0</string>
    <key>CFBundleShortVersionString</key>
    <string>0.9.0-beta</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>IntelliGrade.App</string>
    <key>CFBundleIconFile</key>
    <string>IntelliGrade.icns</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
</dict>
</plist>
EOF

echo -n "APPL????" > "$APP_BUNDLE/Contents/PkgInfo"

echo ""
echo "🔐 Code signing the app..."
SIGN_IDENTITY="Apple Development: Jessen Louis James Forbush (NR5LD2B696)"

# Sign all dylibs and executables first
find "$APP_BUNDLE/Contents/MacOS" -type f \( -name "*.dylib" -o -perm +111 \) -exec codesign --force --sign "$SIGN_IDENTITY" --timestamp --options runtime --entitlements entitlements.plist {} \; 2>&1 | grep -v "code object is not signed at all"

# Deep sign the entire app bundle with entitlements
codesign --force --deep --sign "$SIGN_IDENTITY" --timestamp --options runtime --entitlements entitlements.plist "$APP_BUNDLE"

# Verify the signature
codesign --verify --verbose "$APP_BUNDLE"
if [ $? -eq 0 ]; then
    echo "✅ App successfully signed"
else
    echo "❌ Code signing failed"
    exit 1
fi

echo ""
echo "📀 Creating DMG installer..."

# Create temporary DMG directory
mkdir -p "$DMG_TEMP"
cp -r "$APP_BUNDLE" "$DMG_TEMP/"

# Create Applications symlink
ln -s /Applications "$DMG_TEMP/Applications"

# Create a temporary DMG
echo "  Creating temporary DMG..."
hdiutil create -volname "IntelliGrade" \
    -srcfolder "$DMG_TEMP" \
    -ov -format UDRW \
    -fs HFS+ \
    temp.dmg

# Mount the temporary DMG
echo "  Mounting DMG..."
DEVICE=$(hdiutil attach -readwrite -noverify -noautoopen temp.dmg | \
         grep -E '^/dev/' | sed 1q | awk '{print $1}')

sleep 2

# Set DMG window appearance
echo "  Setting DMG appearance..."
echo '
   tell application "Finder"
     tell disk "IntelliGrade"
           open
           set current view of container window to icon view
           set toolbar visible of container window to false
           set statusbar visible of container window to false
           set the bounds of container window to {400, 100, 1000, 500}
           set viewOptions to the icon view options of container window
           set arrangement of viewOptions to not arranged
           set icon size of viewOptions to 100
           set position of item "IntelliGrade.app" of container window to {150, 200}
           set position of item "Applications" of container window to {450, 200}
           close
           open
           update without registering applications
           delay 2
     end tell
   end tell
' | osascript || true

sleep 2

# Unmount and cleanup
echo "  Finalizing DMG..."
chmod -Rf go-w /Volumes/IntelliGrade 2>/dev/null || true
sync
hdiutil detach "$DEVICE" || true

# Convert to compressed final DMG
hdiutil convert temp.dmg -format UDZO -imagekey zlib-level=9 -o "$DMG_NAME"

# Sign the DMG
echo "  Signing DMG..."
codesign --force --sign "$SIGN_IDENTITY" --timestamp "$DMG_NAME"

# Verify DMG signature
codesign --verify --verbose "$DMG_NAME"
if [ $? -eq 0 ]; then
    echo "  ✅ DMG successfully signed"
else
    echo "  ❌ DMG signing failed"
fi

# Cleanup
rm -f temp.dmg
rm -rf "$DMG_TEMP"

echo ""
echo "✅ Build complete!"
echo ""
echo "📦 App bundle: $APP_BUNDLE"
echo "📀 DMG installer: $DMG_NAME"
echo ""
echo "🚀 Ready to distribute!"
echo ""
echo "To test locally:"
echo "  open $APP_BUNDLE"
echo ""
echo "To install from DMG:"
echo "  open $DMG_NAME"
echo ""
