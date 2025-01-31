#define executable "Belin.Cli.exe"
#define name "Belin.io CLI"
#define root ".."
#define version "2.0.0"

[Setup]
AppCopyright = © Cédric Belin
AppId = {{91CBFC33-9A99-4B9F-8A8D-5B900874EFBF}
AppName = {#name}
AppPublisher = Belin.io
AppPublisherURL = https://belin.io
AppVersion = {#version}
ArchitecturesAllowed = x64compatible
ArchitecturesInstallIn64BitMode = x64compatible
DefaultDirName = {autopf}\{#name}
DisableProgramGroupPage = yes
LicenseFile = {#root}\LICENSE.md
OutputBaseFilename = belin-cli-{#version}
OutputDir = {#root}\var
PrivilegesRequired = lowest
SetupIconFile = {#root}\res\favicon.ico
SolidCompression = yes
UninstallDisplayIcon = {app}\bin\{#executable}
WizardStyle = modern

[Files]
Source: "{#root}\*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#root}\bin\*"; DestDir: "{app}\bin"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#name}"; Filename: "{app}\bin\{#executable}"
Name: "{autodesktop}\{#name}"; Filename: "{app}\bin\{#executable}"; Tasks: desktopicon

[Run]
Filename: "{app}\bin\{#executable}"; Description: "{cm:LaunchProgram,{#StringChange(name, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
