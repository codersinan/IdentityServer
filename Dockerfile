FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
LABEL autodelete="true"
WORKDIR .

COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done

COPY . .

RUN dotnet restore

RUN dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,lcov,json\" /p:CoverletOutput=../lcov /p:MergeWith="../lcov.json"

RUN dotnet publish ./IdentityServer.Api/IdentityServer.Api.csproj -o /publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /publish
COPY --from=build /publish .

ENV ASPNETCORE_URLS="http://+:5000"
ENTRYPOINT ["dotnet","IdentityServer.Api.dll"]
