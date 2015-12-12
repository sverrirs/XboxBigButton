; *************************************************************************
;   This is the NSIS (http://nsis.sf.net) installer for XboxBigButton     *
;                                                                         *
;   This program is free software; you can redistribute it and/or modify  *
;   it under the terms of the GNU General Public License as published by  *
;   the Free Software Foundation; either version 2 of the License, or     *
;   (at your option) any later version.                                   *
;                                                                         *
;   This program is distributed in the hope that it will be useful,       *
;   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
;   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
;   GNU General Public License for more details.                          *
;                                                                         *
;   You should have received a copy of the GNU General Public License     *
;   along with this program; if not, write to the                         *
;   Free Software Foundation, Inc.,                                       *
;   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
; *************************************************************************
;   adapted from https://github.com/fransschreuder/usbpicprog/blob/master/trunk/upp_wx/win/installer/setup.nsi

; -------------------------------------------------------------------------------------------------
; Include Modern UI

;  !include "MUI.nsh"
  !include "MUI2.nsh"
  !include "WinVer.nsh"
  !include "x64.nsh"
  !include "LogicLib.nsh"
  
  
; -------------------------------------------------------------------------------------------------
; This Macro will create a Internet Shorcut (*.url).
; http://nsis.sourceforge.net/Create_Internet_Shorcuts_during_installation
; Well, you might created first then compile it.
; But, let's create a few during installation. :)
; This example put the Internet Shortcut in the $EXEDIR
; with a shortcut in the $DESKTOP.
; Modify the macro according to your needs.
; Created by Joel
; Notes:
; URLFile = The name of our .url file.
; URLSite = The url to the site.
; URLDesc = The description of our shortcut, when mouse hoover it.

!Macro "CreateURL" "URLFile" "URLSite" "URLDesc"
  WriteINIStr "$INSTDIR\${URLFile}.url" "InternetShortcut" "URL" "${URLSite}"
  SetShellVarContext "all"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\${URLFile}.lnk" "$INSTDIR\${URLFile}.url" "" \
                 "$INSTDIR\${URLFile}.url" 0 "SW_SHOWNORMAL" "" "${URLDesc}"
!macroend

!macro "CreateURLShortCut" "URLFile" "URLSite" "URLDesc"
  WriteINIStr "$INSTDIR\${URLFile}.url" "InternetShortcut" "URL" "${URLSite}"
!macroend


; -------------------------------------------------------------------------------------------------
; This macro triggers a scan for hardware change in the Windows Device manager
; From: http://nsis.sourceforge.net/Scan_for_hardware_changes

!define CM_LOCATE_DEVNODE_NORMAL 0x00000000
!define CM_REENUMERATE_NORMAL 0x00000000
!define CR_SUCCESS 0x00000000
 
!macro scanForHardwareChanges
  ; Used to store the last return code
  Push $1             ; Stack: $2
  ; Used for temporary storage of DevInst handle
  Push $0             ; Stack: $0 $1
 
  ; Locate the top node (null)
  System::Call "cfgmgr32::CM_Locate_DevNode(*i.r0, n, i${CM_LOCATE_DEVNODE_NORMAL}) i.r1"
 
  ; Check for success
  ${If} $1 <> ${CR_SUCCESS}
      ; If not successful, push '1' to stack to indicate error in Locate
      Push 1          ; Stack: 1 $0 $1
  ${Else}
    ; Otherwise, Re-enumerate the devices.  This can take a few seconds
    System::Call "cfgmgr32::CM_Reenumerate_DevNode(ir0, i${CM_REENUMERATE_NORMAL}) i.r1"
 
    ; Check for success.
    ${If} $1 <> ${CR_SUCCESS}
        ; If not successful, push '2' to stack to indicate error in Re-enumate
        Push 2      ; Stack: 2 $0 $1
    ${Else}
        ; Otherwise, push '0' to stack to indicate no error encountered
        Push 0
    ${EndIf}
  ${EndIf}
                      ; Stack: <result> $0 $1
  Exch                ; Stack: $0 <result> $1
  Pop $0              ; Stack: <result> $1
  Exch                ; Stack: $1 <result>
  Exch $1             ; Stack: <return> <result>
  Exch                ; Stack: <result> <return>
!macroend
!define scanForHardwareChanges `!insertmacro scanForHardwareChanges`

; -------------------------------------------------------------------------------------------------
; General

  CRCCheck On
  
  ; Overwrite the default ICON in the installer
  !define MUI_ICON "..\xboxbigbutton.ico"

  ; NOTE: the version should be the same as the one in xboxbigbuttonwinusbdriver.inf
  !define OUTPUT_DIR		      "..\binaries\"
  !define PRODUCT_NAME            "Xbox 360 Big Button IR Controller for Windows"
  !define PRODUCT_VERSION         "1.0.0"
  !define PRODUCT_PUBLISHER       "Sverrir Sigmundarson"
  !define PRODUCT_WEBSITE		  "http://sverrirs.github.io/XboxBigButton"
  !define PRODUCT_WEBSITE_TITLE	  "XboxBigButton.NET Website"
  !define PRODUCT_WEBSITE_DESC	  "Visit the XboxBigButton.NET Website for more information and examples"
  !define HARDWARE_ID             "USB\VID_045e&PID_02a0"

  ; are we packaging the 32bit or the 64bit version of the usbpicprog?
  ; allowed values: "x86" or "amd64"
  !ifndef ARCH                     ; see build_installers.bat
    !define ARCH                  "amd64"
  !else
    !if "${ARCH}" != "amd64" 
      !if "${ARCH}" != "x86"
        !error "Invalid value for the ARCH define"
      !endif
    !endif
  !endif

  ; Name and file
  Name "${PRODUCT_NAME} ${PRODUCT_VERSION} ${ARCH} Installer"
  OutFile "${OUTPUT_DIR}${PRODUCT_NAME}-${ARCH}-${PRODUCT_VERSION}.exe" 
  Icon "${MUI_ICON}"

  ; Default installation folder
  !if "${ARCH}" == "amd64"
    InstallDir "$PROGRAMFILES64\${PRODUCT_NAME}"
  !else
    InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"
  !endif
  
  LicenseData "..\..\LICENSE"
  SetCompressor /SOLID lzma    ; this was found to be the best compressor
  
  ; see http://nsis.sourceforge.net/Shortcuts_removal_fails_on_Windows_Vista for more info:
  RequestExecutionLevel admin

; -------------------------------------------------------------------------------------------------
; Pages

  ; Do not automatically jump to the finish page, to allow the user to check the install log.
  !define MUI_FINISHPAGE_NOAUTOCLOSE

  !insertmacro MUI_PAGE_LICENSE "..\..\LICENSE"
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !define MUI_FINISHPAGE_LINK "${PRODUCT_WEBSITE_TITLE}"
  !define MUI_FINISHPAGE_LINK_LOCATION "${PRODUCT_WEBSITE}"
  !insertmacro MUI_PAGE_FINISH
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
; -------------------------------------------------------------------------------------------------
; Interface Settings

  !define MUI_PRODUCT "${PRODUCT_NAME}"
  !define MUI_VERSION "${PRODUCT_VERSION}"
  !define MUI_BRANDINGTEXT "${PRODUCT_NAME} v${PRODUCT_VERSION} (${ARCH})"
  
  ; Warning dialog to confirm that the user indeed want's to abort if Cancel is clicked
  !define MUI_ABORTWARNING
  
  ; show the details view by default
  ShowInstDetails "show"
  ShowUninstDetails "show"
  
; -------------------------------------------------------------------------------------------------
; Languages
 
  !insertmacro MUI_LANGUAGE "English"
  
; -------------------------------------------------------------------------------------------------
; Additional info (will appear in the "details" tab of the properties window for the installer)

  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName"      "${PRODUCT_NAME}"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments"         ""
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName"      "${PRODUCT_NAME} Team"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks"  "Application released under the MIT license"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright"   "© ${PRODUCT_NAME} Team"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription"  "Sverrir Sigmundarson"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion"      "${PRODUCT_VERSION}"
  VIProductVersion                                         "${PRODUCT_VERSION}.0" 


; -------------------------------------------------------------------------------------------------
;  http://nsis.sourceforge.net/Auto-uninstall_old_before_installing_new
Function .onInit
  
  DetailPrint "Searching for older installation of driver"
 ; Select correct registry view before using ReadRegStr
  !if "${ARCH}" == "amd64"
    SetRegView 64
  !else
    SetRegView 32
  !endif
  
  ReadRegStr $R0 HKLM \
  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
  "UninstallString"
  StrCmp $R0 "" no_prev_uninstaller_found
 
  DetailPrint "Found older install, prompt for uninstall of old before installing new"
  
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
  "${PRODUCT_NAME} is already installed. $\n$\nClick `OK` to remove the \
  previous version or `Cancel` to cancel this upgrade." \
  IDOK uninst
  Abort
 
;Run the uninstaller
uninst:
  ClearErrors
  ExecWait '$R0 _?=$INSTDIR' ;Do not copy the uninstaller to a temp file
 
  IfErrors no_remove_uninstaller done
	
no_prev_uninstaller_found:
  DetailPrint "No older installation found"
  Goto done
  
no_remove_uninstaller:
  ;You can either use Delete /REBOOTOK in the uninstaller or add some code
  ;here to remove the uninstaller. Use a registry key to check
  ;whether the user has chosen to uninstall. If you are using an uninstaller
  ;components page, make sure all sections are uninstalled.
 
done:
 
FunctionEnd
  
; -------------------------------------------------------------------------------------------------
; Installer Sections

Section "" ; No components page, name is not important

  DetailPrint "Checking if program is running with Administration privileges"
  
  # call UserInfo plugin to get user info.  The plugin puts the result in the stack
  UserInfo::getAccountType
  Pop $0
  ;DetailPrint "AccountType: $0"
  
  # compare the result with the string "Admin" to see if the user is admin.
  ${If} $0 == "Admin"
    DetailPrint "Program is running with Admin rights"
  ${Else}
    DetailPrint "Program is not running with Admin rights, cannot install driver"
    SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
    Abort "This program needs to be run with Administration rights to be able to install the drivers correctly"
  ${Endif}

  ; check if the architecture of the currently-running system is OK
  ${If} ${RunningX64}
	DetailPrint "Detected a 64 bit Windows architecture"
	${If} "${ARCH}" == "x86"
      MessageBox MB_OK|MB_ICONEXCLAMATION "This installer is for 64bit Windows versions but has detected a 32bit operating system! Please download the x86 version of this installer."
      Quit
	${EndIf}
	Goto check_winversion
	
  ${Else}
	DetailPrint "Detected a 32 bit Windows architecture"
	${If} "${ARCH}" == "amd64"
      MessageBox MB_OK|MB_ICONEXCLAMATION "This installer is for 32bit Windows versions but has detected a 64bit operating system! Please download the AMD64 version of this installer."
      Quit
	${EndIf}
	Goto check_winversion
	
  ${EndIf}
  
check_winversion:
  
  ; Check the windows version, XP and older are not supported
  ${If} ${AtMostWinXP}
    MessageBox MB_OK|MB_ICONEXCLAMATION "This installer is for Windows Vista or newer. Windows XP and older are not supported."
    Quit
  ${EndIf}

proceed:

  ; Set files to be extracted in the user-chosen installation path:

  SetOutPath "$INSTDIR"
  File "${PRODUCT_WEBSITE_TITLE}.url"
  File ..\..\LICENSE
  File ${MUI_ICON}    ; used by the DPINST utility

  SetOutPath "$INSTDIR\driver"
  File ..\driver\xboxbigbuttonwinusbdriver.inf
  File ..\driver\${ARCH}\dpinst.exe  ; Used to install the driver
  File ..\driver\${ARCH}\devcon.exe  ; Attempted driver uninstall, doesn't work due to some strange reason from NSIS (needs elevated cmd and suspect that is not happening)
  File ..\driver\uninstall_device.bat ; Used to uninstall the device from the device manager and capture the return code
  File ..\driver\dpinst.xml          ; Configuration for the installation of the driver

  SetOutPath "$INSTDIR\driver\${ARCH}"
  File ..\driver\${ARCH}\*.dll
  
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  ; Select correct registry view before using WriteRegStr
  !if "${ARCH}" == "amd64"
    SetRegView 64
  !else
    SetRegView 32
  !endif
  
  ; Add the uninstaller to the list of programs accessible from "Control Panel -> Programs and Features"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "DisplayName" "${PRODUCT_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "DisplayIcon" "$\"$INSTDIR\xboxbigbutton.ico$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "URLInfoAbout" "${PRODUCT_WEBSITE}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                   "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
                     "EstimatedSize" 400
                     ; the estimated must be expressed in Kb; for us it's about 400 Kb!

  ; Create shortcuts
  DetailPrint "Creating start menu entries and shortcuts"
  SetShellVarContext all        ; see http://nsis.sourceforge.net/Shortcuts_removal_fails_on_Windows_Vista
  SetOutPath "$INSTDIR"         ; this will be the working directory for the shortcuts created below
  CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
  !insertmacro "CreateURL" "${PRODUCT_WEBSITE_TITLE}" "${PRODUCT_WEBSITE}" "${PRODUCT_WEBSITE_DESC}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
  
  ;${If} ${AtMostWin7}
    ;DetailPrint "Windows version is lower than 7, installing driver"
    ; Last, run the Microsoft driver installer redistributable
    DetailPrint "Running the dpinst utility to install XboxBigButton's drivers"
    ExecWait '"$INSTDIR\driver\dpinst.exe"' $0
    IntFmt $2 "0x%08X" $0
    DetailPrint "Return code was $2"
    ; check the higher byte of the return value of DPINST; it can assume the values:
    ; 0x80 if a driver package could NOT be installed
    ; 0x40 if a computer restart is necessary
    ; 0x00 if everything was ok
    ; or a combination of them (the only possible one in this case is 0xC0)
    ; see http://msdn.microsoft.com/en-us/library/ms791066.aspx for more info
    IntOp $1 $0 / 0x1000000                  ; fast way to keep only the higher byte
    IntFmt $2 "0x%X" $1
    DetailPrint "The higher byte of the return code was $2"
    IntCmp $1 0x00 installed_ok
    IntCmp $1 0x40 installed_ok_need_reboot
    IntCmp $1 0x80 install_failed
    IntCmp $1 0xC0 install_failed
  
    ; unhandled return code ?!?
    DetailPrint "Unknown return value of the DPINST utility! Check %SYSTEMROOT%\DPINST.LOG for more info."
    MessageBox MB_OK|MB_ICONEXCLAMATION "Unknown return value of the DPINST utility! Check %SYSTEMROOT%\DPINST.LOG for more info."
    Abort "Couldn't install drivers! Check %SYSTEMROOT%\DPINST.LOG for more info."
	installed_ok_need_reboot:
	  DetailPrint "Drivers were installed successfully but require a reboot"
	  MessageBox MB_YESNO|MB_ICONQUESTION "The driver installation finished but requires a system reboot. Do you wish to reboot now?" IDNO +2
	  Reboot
	  Goto installed_ok
	install_failed:
	  DetailPrint "Drivers could not be installed! Check %SYSTEMROOT%\DPINST.LOG for more info."
	  MessageBox MB_OK|MB_ICONEXCLAMATION "Couldn't install drivers! Check %SYSTEMROOT%\DPINST.LOG for more info."
	  Abort "Couldn't install drivers! Check %SYSTEMROOT%\DPINST.LOG for more info."
	installed_ok:
	  ; do nothing
	  DetailPrint "Drivers were installed successfully."	
    
  ;${Else}
  ;  DetailPrint "Windows 8 or later detected, use Zadig to install the driver."
  ;  MessageBox MB_OK "Windows 8 or later needs a signed driver.$\r$\nPlease follow the instructions to install the driver$\r$\nA browswer will be opened."
  ;  Exec "rundll32 url.dll,FileProtocolHandler ${PRODUCT_WEBSITE}"
  ;${EndIf} 
  
  ; Finally scan for hardware changes 
  DetailPrint "Attempting to install driver for currently connected Big Button IR hardware"
  ${scanForHardwareChanges}
  Pop $0 ; macro result code
  Pop $1 ; last call return code
  ${If} $0 == 0 
	DetailPrint "Device driver was successfully installed for hardware"
  ${Else}
    DetailPrint "An error occurred while installing driver for hardware: $0 (cfgmgr32.h error: $1)"
  ${EndIf}
  
  
SectionEnd

; -------------------------------------------------------------------------------------------------
; Uninstaller Section

Section "Uninstall"

  DetailPrint "Checking if program is running with Administration privileges"
  
  # call UserInfo plugin to get user info.  The plugin puts the result in the stack
  UserInfo::getAccountType
  Pop $0
  DetailPrint "AccountType: $0"
  
  # compare the result with the string "Admin" to see if the user is admin.
  ${If} $0 == "Admin"
    DetailPrint "Program is running with Admin rights"
  ${Else}
    DetailPrint "Program is not running with Admin rights, cannot install driver"
    SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
    Abort "This program needs to be run with Administration rights to be able to install the drivers correctly"
  ${Endif}
  
  ; note that uninstalling the drivers installed by dpinst.exe is not easy 
  ; and currently it seems to be supported only through the "Add Programs and Features"
  ; panel of Control panel (see http://msdn.microsoft.com/en-us/library/ms791069.aspx),
  ; which we disable in dpinst.xml!
  ; However we can use the PnPutil tool to remove the package and then the 
  ; devcon tool to unload any installed USB devices
  
  ; Select correct registry view before using DeleteRegKey
  !if "${ARCH}" == "amd64"
    SetRegView 64
  !else
    SetRegView 32
  !endif
  
  DetailPrint "Running the pnputil utility to uninstall XboxBigButton's drivers"
  
  ${DisableX64FSRedirection}
  ExecWait '"$WINDIR\system32\PnPutil.exe -f -d"' $0
  ${EnableX64FSRedirection}
  IntFmt $2 "0x%08X" $0
  DetailPrint "Return 2 code was $2"
  ;IntCmp $1 0x00 uninstalled_ok
  ;IntCmp $1 0x40 uninstalled_ok_need_reboot
  ;IntCmp $1 0x80 uninstall_failed
  ;IntCmp $1 0xC0 uninstall_failed
  
  DetailPrint "Removing any installed devices from the Device Manager"
  
  ; Set working dir to the driver dir (otherwise the uninstall of any loaded devices wont work)
  Push $OUTDIR
  SetOutPath "$INSTDIR\driver"

  ; Execute the device uninstall and inf deletion script
  ExecWait '"$INSTDIR\driver\uninstall_device.bat"' $0
  DetailPrint "Return code was $0"
  
  ; Restore the working directory
  Pop $OUTDIR
  SetOutPath $OUTDIR # Optional if working directory does not matter for the rest of the code  
  
  DetailPrint "Device was successfully removed from the System."	
  DetailPrint "Drivers were uninstalled successfully."	
    
  ; clean the list accessible from "Control Panel -> Programs and Features"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"

  ; clean start menu
  SetShellVarContext all
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_WEBSITE_TITLE}.lnk"
  RMDir "$SMPROGRAMS\${PRODUCT_NAME}"

  ; clean installation folder
  Delete "$INSTDIR\uninstall.exe"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\${PRODUCT_WEBSITE_TITLE}.url"
  Delete "$INSTDIR\xboxbigbutton.ico"
  Delete "$INSTDIR\driver\*.inf"
  Delete "$INSTDIR\driver\devcon.exe"
  Delete "$INSTDIR\driver\dpinst.exe"
  Delete "$INSTDIR\driver\dpinst.xml"
  Delete "$INSTDIR\driver\uninstall_device.bat"
  Delete "$INSTDIR\driver\${ARCH}\*.dll"
  RMDir "$INSTDIR\driver\${ARCH}"
  RMDir "$INSTDIR\driver"
  RMDir "$INSTDIR"
  
  DetailPrint "Files deleted successfully."	
  DetailPrint "Uninstall complete."	

SectionEnd

