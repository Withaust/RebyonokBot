@echo off
:start
start /w Godot\\Godot.exe --no-window --path . | set /P "="
goto start
