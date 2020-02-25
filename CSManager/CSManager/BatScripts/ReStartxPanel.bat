@echo off
taskkill /f /t /im xpanel.exe
echo kill xPanel
timeout /t 5 /nobreak
start /d"C:\Himalaya\xPanel\" xPanel.exe
echo start rexPanel