FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && apt-get install -y curl
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
COPY . .
RUN dotnet restore "Whisper.sln"

FROM restore AS build
RUN dotnet build "Whisper.sln" -c Release --no-restore

FROM build AS tested
RUN dotnet test  -c Release --no-build

FROM tested AS publish
RUN dotnet publish "src/Whisper/Whisper.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Whisper.dll"]
