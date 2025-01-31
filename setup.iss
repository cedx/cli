#define executable "Belin.Cli.exe"
#define name "Belin.io CLI"
#define version "2.0.0"

[Setup]
AppCopyright = © Cédric Belin
AppId = {{1EBFF7A9-5220-4538-8758-90A18073C2C4}
AppName = {#name}
AppPublisher = Belin.io
AppPublisherURL = https://belin.io
AppVersion = {#version}
ArchitecturesAllowed = x64compatible
ArchitecturesInstallIn64BitMode = x64compatible
DefaultDirName = {autopf}\{#name}
DisableProgramGroupPage = yes
LicenseFile = LICENSE.md
OutputBaseFilename = belin-cli-{#version}
OutputDir = var
PrivilegesRequired = lowest
SetupIconFile = res\favicon.ico
SolidCompression = yes
UninstallDisplayIcon = {app}\bin\{#executable}
WizardStyle = modern

[Files]
Source: "*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\*"; DestDir: "{app}\bin"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{autoprograms}\{#name}"; Filename: "{app}\bin\{#executable}"
Name: "{autodesktop}\{#name}"; Filename: "{app}\bin\{#executable}"; Tasks: desktopicon

[Run]
Filename: "{app}\bin\{#executable}"; Description: "{cm:LaunchProgram,{#StringChange(name, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
