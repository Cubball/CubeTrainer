FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./CubeTrainer.API.csproj ./CubeTrainer.API.csproj
RUN dotnet restore ./CubeTrainer.API.csproj

COPY . .
RUN dotnet publish -c Release -o ./app ./CubeTrainer.API.csproj --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /src/app/ .
ENTRYPOINT ["dotnet", "CubeTrainer.API.dll"]
