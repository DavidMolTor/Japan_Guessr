rem ----------------------------------------------------------------------------
rem 
rem Project: EXEIRA_GUI
rem
rem Usage: PostBuild [DEBUG | RELEASE] [ProjectRoot] [OutputDir]
rem
rem The first parameter indicates the configuration
rem The second parameter indicates the project root directory
rem The third parameter indicates the output directory
rem
rem ----------------------------------------------------------------------------

rem ----------------------------------------------------------------------------
rem ---- ARGUMENTS CHECKING
rem ----------------------------------------------------------------------------

if %1==Debug         	goto DEBUG
if %1==Release       	goto RELEASE

echo Error during PostBuild step: Invalid configuration - %1
goto END

:RELEASE

rem ----------------------------------------------------------------------------
rem ---- DIRECTORIES
rem ----------------------------------------------------------------------------

set PROJECT_ROOT=%2
set OUT_FOLDER=%3

rem ----------------------------------------------------------------------------
rem ---- REMOVE FILES
rem ----------------------------------------------------------------------------

del "%OUT_FOLDER%\*.pdb" /S /Q >NUL
del "%OUT_FOLDER%\*.metagen" /S /Q >NUL

rem ----------------------------------------------------------------------------
rem ---- MOVE LIBRARIES
rem ----------------------------------------------------------------------------

mkdir "%OUT_FOLDER%\dll"

move "%OUT_FOLDER%\*.dll" 			"%OUT_FOLDER%\dll" >NUL
move "%OUT_FOLDER%\*.xml" 			"%OUT_FOLDER%\dll" >NUL

rem ----------------------------------------------------------------------------
rem ---- COPY GENERATED FILES TO TARGET DIRECTORY
rem ----------------------------------------------------------------------------

rmdir "%PROJECT_ROOT%\..\..\install\JapanGuessr" /S /Q >NUL
mkdir "%PROJECT_ROOT%\..\..\install\JapanGuessr"
xcopy "%OUT_FOLDER:~0,-1%" "%PROJECT_ROOT%\..\..\install\JapanGuessr\" /S /E /Y >NUL

goto END

:DEBUG

rem ----------------------------------------------------------------------------
rem ---- DIRECTORIES
rem ----------------------------------------------------------------------------

set PROJECT_ROOT=%2
set OUT_FOLDER=%3

goto END

:END
echo Postbuild done!
