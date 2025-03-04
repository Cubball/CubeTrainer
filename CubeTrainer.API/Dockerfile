FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./CubeTrainer.Cube/CubeTrainer.Cube.csproj ./CubeTrainer.Cube/CubeTrainer.Cube.csproj
COPY ./CubeTrainer.API/CubeTrainer.API.csproj ./CubeTrainer.API/CubeTrainer.API.csproj
RUN dotnet restore ./CubeTrainer.API/CubeTrainer.API.csproj

COPY ./CubeTrainer.Cube/ ./CubeTrainer.Cube/
COPY ./CubeTrainer.API/ ./CubeTrainer.API/
RUN dotnet publish -c Release -o ./app ./CubeTrainer.API/CubeTrainer.API.csproj --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /src/app/ .
ENTRYPOINT ["dotnet", "CubeTrainer.API.dll"]
