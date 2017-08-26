set artifacts=%~dp0artifacts

if exist %artifacts%  rd /q /s %artifacts%

call dotnet restore src/Literate

call dotnet build src/Literate -f netstandard1.3 -c Release -o %artifacts%\netstandard1.3
call dotnet build src/Literate -f netstandard2.0 -c Release -o %artifacts%\netstandard2.0

call dotnet build src/Literate -f net45 -c Release -o %artifacts%\net45

call dotnet pack src/Literate -c release -o %artifacts%
