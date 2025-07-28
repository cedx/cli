#define executable "Belin.Cli.exe"
#define publisher "Belin.io"
#define name publisher + " CLI"
#define version "2.5.0"

[Setup]
AppCopyright = © Cédric Belin
AppId = {{1EBFF7A9-5220-4538-8758-90A18073C2C4}
AppName = {#name}
AppPublisher = {#publisher}
AppPublisherURL = https://belin.io
AppVersion = {#version}
ArchitecturesAllowed = x64compatible
ArchitecturesInstallIn64BitMode = x64compatible
DefaultDirName = {autopf}\{#publisher}\CLI
DisableProgramGroupPage = yes
LicenseFile = License.md
OutputBaseFilename = {#name} {#version}
OutputDir = var
PrivilegesRequired = admin
PrivilegesRequiredOverridesAllowed = dialog
SetupIconFile = src\Program.ico
SolidCompression = yes
UninstallDisplayIcon = {app}\bin\{#executable}
WizardStyle = modern

[Files]
Source: "*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\*"; DestDir: "{app}\bin"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs
Source: "res\*"; DestDir: "{app}\res"; Flags: ignoreversion recursesubdirs
Source: "run.ps1"; DestDir: "{app}"; DestName: "belin.cli.ps1"; Flags: ignoreversion
