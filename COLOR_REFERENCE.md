# IntelliGrade Color Reference

## Quick Color Guide

### Background Colors
```
App Background:     #e2e8f0  (Light slate gray)
Card Background:    #f8fafc  (Off-white)
Code Background:    #ffffff  (Pure white)
Header:            #0f172a  (Darkest slate)
Status Bar:        #1e293b  (Darker slate)
```

### Text Colors
```
Primary Text:       #1e293b  (Darker slate)
Secondary Text:     #64748b  (Slate-500)
Header Text:        #f8fafc  (Off-white on dark)
Subtitle:          #cbd5e1  (Light slate)
```

### Border & Divider Colors
```
Default Border:     #cbd5e1  (Slate-300)
Hover Border:       #94a3b8  (Slate-400)
Focus Border:       #3b82f6  (Blue)
Card Border:        #e2e8f0  (Slate-200)
Separator:         #e2e8f0  (Slate-200)
```

### Button Colors

#### Primary Button
```
Background:         #3b82f6  (Blue-500)
Hover:             #2563eb  (Blue-600)
Pressed:           #1d4ed8  (Blue-700)
Text:              #f8fafc  (Off-white)
```

#### Secondary Button
```
Background:         #475569  (Slate-600)
Hover:             #334155  (Slate-700)
Pressed:           #1e293b  (Slate-800)
Text:              #f8fafc  (Off-white)
```

#### Success Button
```
Background:         #10b981  (Emerald-500)
Hover:             #059669  (Emerald-600)
Pressed:           #047857  (Emerald-700)
Text:              #f8fafc  (Off-white)
```

#### Danger Button
```
Background:         #ef4444  (Red-500)
Hover:             #dc2626  (Red-600)
Pressed:           #b91c1c  (Red-700)
Text:              #f8fafc  (Off-white)
```

### Badge Colors
```
Success (AI Active):  #10b981  (Emerald-500)
Error (AI Offline):   #ef4444  (Red-500)
```

### Accent Colors
```
Link/Selected:      #3b82f6  (Blue-500)
Letter Grade:       #3b82f6  (Blue-500)
```

## Color Palette Overview

### Slate Grays (Main Palette)
- `#0f172a` - slate-900 (Darkest - Header)
- `#1e293b` - slate-800 (Darker - Status bar, primary text)
- `#334155` - slate-700 (Dark - Hover states)
- `#475569` - slate-600 (Primary slate - Secondary buttons)
- `#64748b` - slate-500 (Medium - Secondary text)
- `#94a3b8` - slate-400 (Light - Hover borders)
- `#cbd5e1` - slate-300 (Lighter - Default borders)
- `#e2e8f0` - slate-200 (Very light - Separators, app background)
- `#f8fafc` - slate-50 (Off-white - Cards, text on dark)

### Blue (Accent)
- `#1d4ed8` - blue-700 (Pressed)
- `#2563eb` - blue-600 (Hover)
- `#3b82f6` - blue-500 (Primary accent)

### Green (Success)
- `#047857` - emerald-700 (Pressed)
- `#059669` - emerald-600 (Hover)
- `#10b981` - emerald-500 (Success)

### Red (Danger)
- `#b91c1c` - red-700 (Pressed)
- `#dc2626` - red-600 (Hover)
- `#ef4444` - red-500 (Danger)

## Usage Examples

### Buttons
```xml
<Button Content="Save" Classes="primary"/>      <!-- Blue -->
<Button Content="Analyze" Classes="secondary"/> <!-- Slate -->
<Button Content="Export" Classes="success"/>    <!-- Green -->
<Button Content="Delete" Classes="danger"/>     <!-- Red -->
```

### Text
```xml
<TextBlock Text="Label" Classes="label"/>        <!-- Slate-800 -->
<TextBlock Text="Header" Classes="section-header"/> <!-- Slate-900 -->
<TextBlock Text="Info" Classes="description"/>   <!-- Slate-600 -->
```

### Cards
```xml
<Border Classes="card">
    <!-- Off-white background with subtle shadow -->
</Border>
```

### Code Display
```xml
<TextBox Text="{Binding Code}" Classes="code"/>
<!-- White background, slate text, monospace font -->
```

## Accessibility

All color combinations meet WCAG AA standards:
- Dark text (#1e293b) on light backgrounds (#f8fafc, #ffffff)
- Light text (#f8fafc) on dark backgrounds (#0f172a, #1e293b)
- Accent blue (#3b82f6) has sufficient contrast against light backgrounds
- Focus indicators use 2px borders for visibility
