Steps for releasing XboxBigButton WinUsbDriver on Windows
-----------------------------------------

1) Download and install NSIS v3+ (not version 2).
2) Copy GetVersion.dll in your NSIS\Plugins folder
3) Update the WinVer.nsh file in your NSIS\include folder from the NISIS 3 version (if needed)
   See: http://nsis.sourceforge.net/Get_Windows_version
4) Set ARCH=x86 in setup.nsi and compile it with NSIS; set ARCH=amd64 in setup.nsi and compile it with NSIS.
   Note that you can also use the build_installers.bat batch file to do this step automatically.
5) Upload the two installers on the release file server.
   They should install and work fine on both 32bit and 64bit versions of Windows XP, Windows Vista,
   Windows Server 2003, Windows 7 (and higher).