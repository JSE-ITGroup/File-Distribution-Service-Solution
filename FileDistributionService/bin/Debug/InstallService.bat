REM https://www.codeproject.com/Questions/505250/HowplustoplusInstallplusorplusUninstallplusWindows
@ECHO OFF

REM The following directory is for .NET 2.0
set DOTNET=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNET%

echo Installing MyService...
echo ---------------------------------------------------
InstallUtil /i "D:\Software-Development\Source\Repos\FDS2\FileDistributionServiceSolution\FileDistributionService\bin\Debug\FileDistributionService.exe"
echo ---------------------------------------------------
echo Done.
pause
