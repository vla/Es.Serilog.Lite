#!/usr/bin/env bash
set -e
basepath=$(cd `dirname $0`; pwd)
artifacts=${basepath}/artifacts

if [[ -d ${artifacts} ]]; then
   rm -rf ${artifacts}
fi

mkdir -p ${artifacts}

dotnet restore src/Literate
dotnet restore src/Es.Logging.Console

dotnet build src/Literate -f netstandard1.3 -c Release -o ${artifacts}/netstandard1.3
dotnet build src/Literate -f netstandard2.0 -c Release -o ${artifacts}/netstandard2.0

