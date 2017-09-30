@echo off

mkdir Build-Files\Notifications
mkdir Build-Files\SchickMirDeinFoto

xcopy /s/b/v/y SchatziSchickMirDeinFoto\bin\Release\netcoreapp2.0 Build-Files\SchickMirDeinFoto
xcopy /s/b/v/y SelectionPopup\bin\Release Build-Files\Notifications

DEL /Q Build-Files\SchickMirDeinFoto\*.pdb
DEL /Q Build-Files\Notifications\*.pdb