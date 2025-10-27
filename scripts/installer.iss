; IntelliGrade Inno Setup Installer Script
; This creates a Windows installer for IntelliGrade

#define MyAppName "IntelliGrade"
#define MyAppVersion "0.9.0-beta"
#define MyAppPublisher "IntelliGrade"
#define MyAppURL "https://github.com/JKaizenn/Intelligrade"
#define MyAppExeName "IntelliGrade.App.exe"

[Setup]
; App information
AppId={{E4B9A2F1-8C3D-4E5F-9B1A-7D6C8E2F4A9B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=dist
OutputBaseFilename=IntelliGrade-v{#MyAppVersion}-Windows-Setup
; SetupIconFile=src\IntelliGrade.App\Assets\IG-Icon.ico  ; Uncomment if you have a .ico file
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; Privileges
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

; Visual appearance
DisableProgramGroupPage=yes
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "dist\windows\IntelliGrade.App.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "dist\windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "scripts\launch-debug.bat"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName} (Debug Mode)"; Filename: "{app}\launch-debug.bat"; Comment: "Launch with error diagnostics"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
