#define executable "Belin.Cli.exe"
#define publisher "Belin.io"
#define name publisher + " CLI"
#define version "2.0.0"

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
LicenseFile = LICENSE.md
OutputBaseFilename = belin.cli-{#version}
OutputDir = var
PrivilegesRequired = lowest
SetupIconFile = res\favicon.ico
SolidCompression = yes
UninstallDisplayIcon = {app}\bin\{#executable}
WizardStyle = modern

[Files]
Source: "*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\*"; DestDir: "{app}\bin"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs
Source: "res\file_extensions\*"; DestDir: "{app}\res\file_extensions"; Flags: ignoreversion recursesubdirs
Source: "run.ps1"; DestDir: "{app}"; DestName: "belin.cli.ps1"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#name}"; IconFilename: "{app}\bin\{#executable}"; Filename: "pwsh.exe"; \
	Parameters: "-ExecutionPolicy Bypass -NoExit -NoLogo belin.cli.ps1"; WorkingDir: "{app}"
Name: "{autoprograms}\{#name} (Administrateur)"; IconFilename: "{app}\bin\{#executable}"; Filename: "pwsh.exe"; \
	Parameters: "-ExecutionPolicy Bypass -NoExit -NoLogo belin.cli.ps1"; WorkingDir: "{app}"; \
	AfterInstall: SetElevationBit('{autoprograms}\{#name} (Administrateur).lnk')

[Code]
procedure SetElevationBit(filename: String);
var
	buffer: String;
	stream: TStream;
begin
	filename := ExpandConstant(filename);
	stream := TFileStream.Create(filename, fmOpenReadWrite);
	try
		stream.Seek(21, soFromBeginning);
		SetLength(buffer, 1);
		stream.ReadBuffer(buffer, 1);
		buffer[1] := Chr(Ord(buffer[1]) or $20);
		stream.Seek(-1, soFromCurrent);
		stream.WriteBuffer(buffer, 1);
	finally
		stream.Free;
	end;
end;
