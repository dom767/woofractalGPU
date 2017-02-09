; example2.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

;--------------------------------

; The name of the installer
Name "WooFractal"

; The file to write
OutFile "WooFractal.exe"

; The default installation directory
InstallDir $PROGRAMFILES\WooFractal

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\WooFractal" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "WooFractal (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /oname=coretracer.dll "..\bin\coretracer.dll"
  File /oname=woofractal.exe "..\bin\Release\woofractal.exe"

  SetOutPath $DOCUMENTS

  CreateDirectory $DOCUMENTS\WooFractal\Scripts\Background
  CreateDirectory $DOCUMENTS\WooFractal\Scripts\Lighting

  ; examples
  File /oname=WooFractal\Scripts\Background\scratch.woo "..\Scripts\Background\boxes.woo"
  File /oname=WooFractal\Scripts\Background\plain_white_circle.woo "..\Scripts\Background\plain_white_circle.woo"
  File /oname=WooFractal\Scripts\Background\boxes.woo "..\Scripts\Background\boxes.woo"

  File /oname=WooFractal\Scripts\Lighting\scratch.woo "..\Scripts\Lighting\daylight.woo"
  File /oname=WooFractal\Scripts\Lighting\examples.woo "..\Scripts\Lighting\examples.woo"
  File /oname=WooFractal\Scripts\Lighting\bigsphere.woo "..\Scripts\Lighting\bigsphere.woo"
  File /oname=WooFractal\Scripts\Lighting\daylight.woo "..\Scripts\Lighting\daylight.woo"
  File /oname=WooFractal\Scripts\Lighting\plaindirectional.woo "..\Scripts\Lighting\plaindirectional.woo"
  File /oname=WooFractal\Scripts\Lighting\ringoflight.woo "..\Scripts\Lighting\ringoflight.woo"
  File /oname=WooFractal\Scripts\Lighting\sunset.woo "..\Scripts\Lighting\sunset.woo"

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\WooFractal "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooFractal" "DisplayName" "WooFractal"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooFractal" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooFractal" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooFractal" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\WooFractal"
  CreateShortCut "$SMPROGRAMS\WooFractal\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\WooFractal\WooFractal.lnk" "$INSTDIR\WooFractal.exe" "" "$INSTDIR\WooFractal.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooFractal"
  DeleteRegKey HKLM SOFTWARE\WooFractal

  ; Remove files and uninstaller
  Delete $INSTDIR\coretracer.dll
  Delete $INSTDIR\uninstall.exe

  ; Remove Scripts
;  Delete $DOCUMENTS\WooFractal\Scripts

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\WooFractal\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\WooFractal"
  RMDir "$INSTDIR"

SectionEnd
