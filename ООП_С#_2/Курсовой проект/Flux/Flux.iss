; Inno Setup script для приложения Flux
; Скачай Inno Setup: https://jrsoftware.org/isdl.php
; Открой этот файл в Inno Setup Compiler и нажми "Compile" (F9).
; Перед компиляцией: собери проект в режиме Release и опубликуй self-contained:
;   dotnet publish Flux.csproj -c Release -r win-x64 --self-contained true -o publish
; Затем компилируй этот скрипт — на выходе будет Output\FluxSetup.exe

#define MyAppName "Flux"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Flux Team"
#define MyAppExeName "Flux.exe"

[Setup]
AppId={{A8F7E9B1-3D4A-4B2E-9C8F-1F0E5D6C7B8A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=FluxSetup
SetupIconFile=Resources\flux.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Архитектура. Раньше стояло "x64" — это исключало ARM64-машины (Windows on ARM,
; ноутбуки на Snapdragon), и установщик выдавал «не поддерживает версию Windows».
; "x64compatible" разрешает установку на любой ОС, способной выполнять x64-код
; (обычный x64 И ARM64 с эмуляцией x64). Блок ниже выбирает синтаксис в зависимости
; от версии Inno Setup: новое ключевое слово доступно начиная с 6.3.
#if defined(Ver) && (Ver >= EncodeVer(6,3,0,0))
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#else
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
#endif
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Все файлы из папки publish будут включены
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
