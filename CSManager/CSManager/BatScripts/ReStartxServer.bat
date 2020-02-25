@echo off
taskkill /f /t /im xServer.exe
echo kill xPanel
timeout /t 5 /nobreak
start /d"C:\Himalaya\xPanel\" xServer.exe
echo restart xServer