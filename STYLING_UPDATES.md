# IntelliGrade Styling Updates

## Color Scheme - Slate Gray & Off-White

I've completely redesigned IntelliGrade with a modern, professional slate gray and off-white color palette inspired by Tailwind CSS color system.

### Color Palette

**Primary Colors:**
- **Darkest Slate**: `#0f172a` (slate-900) - Header background
- **Darker Slate**: `#1e293b` (slate-800) - Status bar, text
- **Dark Slate**: `#334155` (slate-700) - Button hover states
- **Primary Slate**: `#475569` (slate-600) - Secondary buttons

**Light Colors:**
- **Off-White**: `#f8fafc` (slate-50) - Cards, text on dark
- **Light Gray**: `#e2e8f0` (slate-200) - App background, separators
- **Medium Gray**: `#cbd5e1` (slate-300) - Borders

**Accent Colors:**
- **Accent Blue**: `#3b82f6` (blue-500) - Primary buttons, links
- **Success Green**: `#10b981` (emerald-500) - Success actions
- **Danger Red**: `#ef4444` (red-500) - Delete/cleanup actions

### Responsive Design Features

#### Window Settings
- **Default Size**: 1400x900px (professional desktop layout)
- **Minimum Size**: 900x600px (ensures usability on smaller screens)
- **Window Behavior**: Centers on screen on startup
- **Background**: Light slate gray (#e2e8f0)

#### Layout Improvements

**1. Flexible Grid System**
- Changed from fixed `360,*` to `Auto,*` for better scaling
- Left sidebar maintains 340px width for consistency
- Right panel expands/contracts with window size

**2. Responsive Spacing**
- Reduced margins from 20px to 16px for better space usage
- Card spacing: 8px between panels
- Internal spacing: 16px for better breathing room

**3. Content Scaling**
- Code editors: Minimum height reduced to 400px (from 500px)
- Allows more content visibility on smaller screens
- ScrollViewers enable vertical scrolling when needed

### Component Styling

#### Buttons
- **Border Radius**: 6px (modern rounded corners)
- **Hover States**: Darker shade on hover for clear feedback
- **Pressed States**: Even darker for tactile feel
- **Disabled States**: Muted gray with reduced opacity
- **Cursor**: Hand pointer for better UX

#### Text Inputs & ComboBoxes
- **Background**: Pure white (#ffffff)
- **Border**: Slate-300 (#cbd5e1) with 1px thickness
- **Focus State**: Blue border (#3b82f6) with 2px thickness
- **Hover State**: Darker gray border (#94a3b8)
- **Corner Radius**: 6px for consistency
- **Padding**: 10px horizontal, 8px vertical

#### Cards
- **Background**: Off-white (#f8fafc)
- **Border**: 1px solid slate-200
- **Corner Radius**: 12px (softer, more modern)
- **Shadow**: Subtle double shadow for depth
  - `0 4 6 -1 #0f172a10` (outer shadow)
  - `0 2 4 -2 #0f172a06` (inner shadow)

#### Typography
- **Labels**: Slate-800, 14px, semibold
- **Headers**: Slate-900, 18px, bold
- **Descriptions**: Slate-600, 13px
- **Code**: JetBrains Mono, Consolas, Monaco, Courier New

#### Tabs
- **Default**: Slate-500 text (#64748b)
- **Selected**: Blue (#3b82f6)
- **Hover**: Darker slate (#475569)
- **Padding**: 16px horizontal, 10px vertical

### Dark Elements

**Header:**
- Background: Darkest slate (#0f172a)
- Title: Off-white (#f8fafc)
- Subtitle: Light slate (#cbd5e1)

**Status Bar:**
- Background: Darker slate (#1e293b)
- Text: Off-white (#f8fafc)
- Version: Muted slate (#64748b)

**Badges:**
- Success (AI Active): Emerald green (#10b981)
- Error (AI Offline): Red (#ef4444)
- Border radius: 16px for pill shape

### Code Editor Styling

**Enhanced Code Display:**
- **Font Stack**: JetBrains Mono → Consolas → Menlo → Monaco → monospace
- **Background**: Pure white (#ffffff)
- **Text**: Slate-800 (#1e293b)
- **Border**: 1px solid slate-300
- **Corner Radius**: 6px
- **Padding**: 12px
- **Min Height**: 400px (responsive)

### Accessibility Features

1. **High Contrast**: Dark text on light backgrounds, light text on dark
2. **Focus Indicators**: Blue 2px borders on focused inputs
3. **Hover States**: All interactive elements have clear hover feedback
4. **Cursor Changes**: Hand cursor on all clickable elements
5. **Readable Font Sizes**: 13-14px for body, larger for headers

### Cross-Device Compatibility

**Desktop (1920x1080+)**
- Full 1400x900 window
- Sidebar: 340px fixed
- Content area: ~1000px+ expandable
- All features visible without scrolling

**Laptop (1366x768)**
- Window scales down gracefully
- Maintains 900x600 minimum
- ScrollViewer enables access to all content
- Sidebar remains fixed width

**Smaller Screens (1280x720)**
- Minimum 900x600 enforced
- Vertical scrolling in both panels
- All functionality accessible
- No horizontal scrolling needed

## Build Status

✅ **Build Successful**
- 0 Errors
- 0 Warnings
- Clean compilation with .NET 9.0

## Files Modified

1. **[Styles/Styles.axaml](src/IntelliGrade.App/Styles/Styles.axaml)** - Complete style system
2. **[Views/MainWindow.axaml](src/IntelliGrade.App/Views/MainWindow.axaml)** - Layout and responsiveness
3. **[App.axaml](src/IntelliGrade.App/App.axaml)** - Style inclusion

## Visual Improvements Summary

✅ Modern slate gray and off-white color scheme
✅ Professional rounded corners (6-12px)
✅ Subtle shadows for depth
✅ Clear focus and hover states
✅ Responsive layout (900-1920px+ width)
✅ Improved typography hierarchy
✅ Better spacing and breathing room
✅ Accessible color contrasts
✅ Consistent border radius across all elements
✅ Enhanced code editor with better fonts

## How to Run

```bash
cd /Users/jforbush/Dev/Intelligrade/Intelligrade
dotnet run --project src/IntelliGrade.App
```

The application will open centered on your screen with the new slate gray and off-white theme!
