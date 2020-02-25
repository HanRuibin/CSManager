@echo off
taskkill /f /t /im xSwitch.exe
echo kill xSwitch
timeout /t 5 /nobreak
start /d"C:\Himalaya\xPanel\" xSwitch.exe
echo restart xSwitch
