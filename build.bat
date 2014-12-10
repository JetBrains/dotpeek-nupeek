@echo off
setlocal enableextensions

set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Clean
echo Cleaning...
del /q src\NuPeek\bin\Release\*

REM Build DotPeek 1.0 version
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\NuPeek.1.0.sln /p:Configuration="%config%" /t:Clean,Rebuild /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
mkdir install\NuPeek.1.0 2> NUL
copy /y src\NuPeek\bin\Release\*.1.0.dll install\NuPeek.1.0\
copy /y src\NuPeek\bin\Release\NuGet.Core.dll install\NuPeek.1.0\

REM Clean
echo Cleaning...
del /q src\NuPeek\bin\Release\*

REM Build DotPeek 1.1 version
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\NuPeek.1.1.sln /p:Configuration="%config%" /t:Clean,Rebuild /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
mkdir install\NuPeek.1.1 2> NUL
copy /y src\NuPeek\bin\Release\*.1.1.dll install\NuPeek.1.1\
copy /y src\NuPeek\bin\Release\NuGet.Core.dll install\NuPeek.1.1\

REM Clean
echo Cleaning...
del /q src\NuPeek\bin\Release\*

REM Build DotPeek 1.2 version
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\NuPeek.1.2.sln /p:Configuration="%config%" /t:Clean,Rebuild /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
mkdir install\NuPeek.1.2 2> NUL
copy /y src\NuPeek\bin\Release\*.1.2.dll install\NuPeek.1.2\
copy /y src\NuPeek\bin\Release\NuGet.Core.dll install\NuPeek.1.2\

REM Build DotPeek 1.3 version
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\NuPeek.1.2.sln /p:Configuration="%config%" /t:Clean,Rebuild /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
mkdir install\NuPeek.1.3 2> NUL
copy /y src\NuPeek\bin\Release\*.1.3.dll install\NuPeek.1.3\
copy /y src\NuPeek\bin\Release\NuGet.Core.dll install\NuPeek.1.3\