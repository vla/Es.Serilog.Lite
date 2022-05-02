set artifacts=%~dp0artifacts
set /p ver=<VERSION

if exist %artifacts%  rd /q /s %artifacts%

call dotnet restore src/Literate

call dotnet pack src/Literate -c release -p:Ver=%ver%  -o %artifacts%

pause