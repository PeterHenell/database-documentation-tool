@echo off

call "C:\Windows\Microsoft.NET\assembly\GAC_64\MSBuild\v4.0_12.0.0.0__b03f5f7f11d50a3a\MSBuild.exe"

for /f "tokens=1* delims=" %%a in ('date /T') do set datestr=%%a

set /p version=<.\released-binaries\release-version.txt
for /F "tokens=1,2,3 delims=. " %%a in ("%version%") do (
   set major=%%a
   set minor=%%b
   set bugfix=%%c
)

set /A minor=minor+1
set newversion=%major%.%minor%.%bugfix%

xcopy .\ExtendedPropertiesDocumentationTool\bin\Release\*.* .\released-binaries\%newversion% /y /i

del .\released-binaries\%newversion%.zip
call "C:\Program Files\7-Zip\7z.exe" a -tzip ".\released-binaries\%newversion%.zip" ".\released-binaries\%newversion%"

rmdir /s /q .\released-binaries\%newversion%

@ECHO *******************************************
@ECHO      ZIP Completed


echo %newversion%>.\released-binaries\release-version.txt