set artifacts=%~dp0artifacts

if exist %artifacts%  rd /q /s %artifacts%

call dotnet restore src/Literate

call dotnet pack src/Literate -c release -o %artifacts%

pause