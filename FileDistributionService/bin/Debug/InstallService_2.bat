REM https://www.codeproject.com/Questions/505250/HowplustoplusInstallplusorplusUninstallplusWindows
@ECHO OFF

echo Installing MyService...

cd C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\

InstallUtil.exe "C:\Data\SourceCodeRepository\FDS\FileDistributionServiceSolution\FileDistributionService\bin\Debug\FileDistributionService.exe"

echo Done.
pause
