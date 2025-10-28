@echo off
REM IntelliGrade Debug Launcher
REM This batch file helps diagnose startup issues by showing error messages

echo Starting IntelliGrade...
echo.
echo If the app doesn't start, you should see an error message below:
echo ================================================================
echo.

"%~dp0IntelliGrade.exe"

if errorlevel 1 (
    echo.
    echo ================================================================
    echo ERROR: Application exited with error code %errorlevel%
    echo.
    echo Please take a screenshot of this window and report the issue at:
    echo https://github.com/JKaizenn/Intelligrade/issues
    echo.
    pause
)
