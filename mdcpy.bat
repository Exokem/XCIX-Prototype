@echo off

set "src=Modules"
set "dst=bin\\Debug\\netcoreapp3.1\\Modules"

rd "%dst%" /s /q
robocopy "%src%" "%dst%" /z /e

if ErrorLevel 8 (exit /b 1) else (exit /b 0)