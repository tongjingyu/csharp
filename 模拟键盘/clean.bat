@echo off
for 
/f "delims=" %%i in ('dir /s/b/ad Backup') do (
        rd /s/q "%%~i"
)exit