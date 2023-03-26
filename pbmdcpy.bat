@echo off

set "src=Modules"
set "dst=bin\\Release\\netcoreapp3.1\\win-x64\\publish\\Modules"

rd "%dst%" /s /q
robocopy "%src%" "%dst%" /z /e

if ErrorLevel 8 (exit /b 1) else (exit /b 0)