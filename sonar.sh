#!/bin/bash
token="$(cat sonar-token.txt)"
dir="$(pwd)"
dotnet sonarscanner begin /k:"IdentityServer" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="${token}" /d:sonar.language="cs" /d:sonar.coverage.exclusions="*.Tests/**,**/*Tests.cs" /d:sonar.cs.opencover.reportsPaths="${dir}/lcov.opencover.xml"
dotnet restore
dotnet build
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,lcov,json\" /p:CoverletOutput=../lcov /p:MergeWith="../lcov.json"
dotnet sonarscanner end /d:sonar.login="${token}"